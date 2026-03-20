using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace MyFirstProject.Extensions;

public static class JwtExtension
{
    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration config)
    {

    services.AddAuthentication(options =>
    {
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
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
    options.ApplyTokenSanitization();
    });
        return services;
    }

    public static JwtBearerOptions ApplyTokenSanitization(this JwtBearerOptions options)
    {
    options.Events = new JwtBearerEvents
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
                if ((tokenPart.StartsWith("\"") && tokenPart.EndsWith("\"")) ||
                    (tokenPart.StartsWith("'") && tokenPart.EndsWith("'")))
                {
                    tokenPart = tokenPart.Substring(1, tokenPart.Length - 2);
                }
                try
                {
                    var decoded = System.Net.WebUtility.UrlDecode(tokenPart);
                    if (!string.IsNullOrEmpty(decoded)) tokenPart = decoded;
                }
                catch { }

                tokenPart = tokenPart.Trim('\r', '\n', '\t', ' ');

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
            Console.WriteLine("[Jwt] Authentication failed: " + context.Exception?.Message);
            return Task.CompletedTask;
        },
        OnTokenValidated = context =>
        {
            Console.WriteLine("[Jwt] Token validated for: " + context.Principal?.Identity?.Name);
            return Task.CompletedTask;
        }
    };
    return options;
}
    }

