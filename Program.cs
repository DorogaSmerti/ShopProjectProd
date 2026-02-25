using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using MyFirstProject.Data;
using MyFirstProject.Services;
using System.Text;
using MyFirstProject.Middleware;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

// Здесь добавляются сервисы, например, контроллеры
builder.Services.AddControllers();
builder.Services.AddControllers(options =>
{
    options.SuppressAsyncSuffixInActionNames = false;
});
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    // Use the classic JwtSecurityTokenHandler to avoid runtime mismatches
    // Compatibility: enable explicit SecurityTokenValidators and use the classic handler
    options.UseSecurityTokenValidators = true;
    options.SecurityTokenValidators.Clear();
    options.SecurityTokenValidators.Add(new JwtSecurityTokenHandler());

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidAudience = config["Jwt:Audience"],
        ValidIssuer = config["Jwt:Issuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!))
    };

    // Minimal, safe sanitization: clean common client-side token formatting issues
    options.Events = new Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var raw = context.Request.Headers["Authorization"].ToString();
            if (!string.IsNullOrEmpty(raw))
            {
                var tokenPart = raw;
                if (raw.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                {
                    tokenPart = raw.Substring(7);
                }
                tokenPart = tokenPart.Trim();
                // Remove surrounding quotes if present
                if ((tokenPart.StartsWith("\"") && tokenPart.EndsWith("\"")) ||
                    (tokenPart.StartsWith("'") && tokenPart.EndsWith("'")))
                {
                    tokenPart = tokenPart.Substring(1, tokenPart.Length - 2);
                }
                // URL decode (handles accidental encoding)
                try
                {
                    var decoded = System.Net.WebUtility.UrlDecode(tokenPart);
                    if (!string.IsNullOrEmpty(decoded)) tokenPart = decoded;
                }
                catch { }

                tokenPart = tokenPart.Trim('\r', '\n', '\t', ' ');

                // If it looks like a JWT (has two dots), set it for the handler
                if (tokenPart.Split('.').Length == 3)
                {
                    context.Token = tokenPart;
                }
            }
            return Task.CompletedTask;
        }
        ,
        OnAuthenticationFailed = context =>
        {
            // Log the failure reason to console to help debugging (temporary)
            Console.WriteLine("[Jwt] Authentication failed: " + context.Exception?.Message);
            return Task.CompletedTask;
        },
        OnTokenValidated = context =>
        {
            Console.WriteLine("[Jwt] Token validated for: " + context.Principal?.Identity?.Name);
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IReviewService, ReviewService>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IWishListItemService, WishListItemService>();
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddStackExchangeRedisCache(option =>
{
    option.Configuration = "localhost:6379";
    option.InstanceName = "MyFirstProject_";
});

builder.Services.AddSwaggerGen(options =>
{
    // 1. Добавляем кнопку "Authorize"
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Введите 'Bearer' [пробел] и ваш токен.\r\n\r\nПример: \"Bearer eyJhbGci...\""
    });

    // 2. Говорим Swagger-у использовать этот токен для всех методов
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});
var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    var roles = new[] { "Admin", "User", "Manager" };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }
}

app.Run();