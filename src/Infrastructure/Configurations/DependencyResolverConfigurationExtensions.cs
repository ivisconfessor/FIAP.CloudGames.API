using System.Text;
using FIAP.CloudGames.API.Infrastructure.Data;
using FIAP.CloudGames.API.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace FIAP.CloudGames.API.Infrastructure.Configurations;

public static class DependencyResolverConfigurationExtensions
{
    public static void IntegrateDependencyResolver(this IServiceCollection services, IConfiguration configuration)
    {
        // Configuração do DbContext
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseInMemoryDatabase("FIAPCloudGames"));

        // Configuração do JWT com definição explícita do esquema padrão
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = configuration["Jwt:Issuer"],
                ValidAudience = configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(configuration["Jwt:Key"]))
            };
        });

        // Configuração da Autorização
        services.AddAuthorization();

        // Registro dos serviços
        services.AddScoped<IJwtService, JwtService>();
    }
} 