using BookingSystem.Application.Interfaces.Appointments;
using BookingSystem.Application.Interfaces.Auth;
using BookingSystem.Application.Interfaces.BlockedHours;
using BookingSystem.Application.Interfaces.Provider;
using BookingSystem.Application.Interfaces.WorkingHours;
using BookingSystem.Infrastructure.Security;
using BookingSystem.Infrastructure.Services.Appointments;
using BookingSystem.Infrastructure.Services.Auth;
using BookingSystem.Infrastructure.Services.BlockedHours;
using BookingSystem.Infrastructure.Services.Provider;
using BookingSystem.Infrastructure.Services.WorkingHour;
using BookingSystem.Infrastructure.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingSystem.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureDI(this IServiceCollection services, IConfiguration configuration)
    {

        //redis cache configuration
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = configuration["Redis:ConnectionString"];
            options.InstanceName = configuration["Redis:InstanceName"];
        });

        //jwt settings
        services.Configure<JwtSettings>(options =>
            configuration.GetSection("JwtSettings").Bind(options));


        //jwt authentication
        var jwtSettings = configuration.GetSection("JwtSettings").Get<JwtSettings>();
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
                 ValidIssuer = jwtSettings!.Issuer,
                 ValidAudience = jwtSettings.Audience,
                 IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings.SecretKey))
             };
         });

        services.AddScoped<ITokenGenerator, TokenGenerator>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IProviderService, ProviderService>();
        services.AddScoped<IWorkingHoursService, WorkingHoursService>();
        services.AddScoped<IBlockedTimeService, BlockedTimeService>();
        services.AddScoped<IAppointmentService, AppointmentService>();

        return services;
    }
}
