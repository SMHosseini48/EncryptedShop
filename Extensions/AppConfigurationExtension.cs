using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MultiLevelEncryptedEshop.Interfaces.Services;
using MultiLevelEncryptedEshop.Services;

namespace MultiLevelEncryptedEshop.Extensions;

public static class AppConfigurationExtension
{
    public static void AddCustomServices(this IServiceCollection services)
    {
        services.AddScoped<IAccountService, AccountService>();
    }

    public static TokenValidationParameters ValidationParameters(IConfiguration configuration, bool expired)
    {
        var sercrKey = Encoding.UTF8.GetBytes(configuration["JWT:Key"]);
        byte[] ecKey = new byte[256 / 8];
        Array.Copy(sercrKey, ecKey, 256 / 8);


        var aliveTokenValidation = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = configuration["JWT:Issuer"],
            ValidAudience = configuration["JWT:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(sercrKey),
            ClockSkew = TimeSpan.Zero,
            TokenDecryptionKey = new SymmetricSecurityKey(ecKey),
        };

        var expiredTokenValidation = aliveTokenValidation;
        expiredTokenValidation.ValidateLifetime = false;


        return expired ? expiredTokenValidation : aliveTokenValidation;
    }

    public static void ConfigureJwt(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication(x =>
        {
            x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(o =>
        {
            o.SaveToken = true;
            o.TokenValidationParameters = ValidationParameters(configuration, false);
        });
    }
}