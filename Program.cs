using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyFirstProject.Data;
using MyFirstProject.Services;
using MyFirstProject.Middleware;
using Serilog;
using MyFirstProject.BackgroundServices;
using MyFirstProject.Extensions;
using FluentValidation.AspNetCore;
using FluentValidation;
using MyFirstProject.Validators;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

var config = builder.Configuration;
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

builder.Services.AddDistributedMemoryCache();

builder.Services.AddJwtAuthentication(config);

builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IReviewService, ReviewService>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IWishListItemService, WishListItemService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddSingleton<IRateLimitStore, RateLimitStore>();

builder.Services.AddValidationCatcher();
builder.Services.AddValidatorsFromAssemblyContaining<CreateProductValidator>();
builder.Services.AddFluentValidationAutoValidation();

builder.Services.AddHostedService<RateLimitCleaner>();

builder.Services.AddSwaggerDocumentation();

var app = builder.Build();

app.UseSwaggerDocumentation();

app.UseMiddleware<ExceptionMiddleware>();
app.UseMiddleware<RateLimitingMiddleware>();
app.UseMiddleware<DecoratorMiddleware>();
app.UseMiddleware<LoggerMiddleware>();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.PrepareDatabaseAsync().Wait();

app.Run();