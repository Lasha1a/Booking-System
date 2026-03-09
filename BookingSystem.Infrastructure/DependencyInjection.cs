using BookingSystem.Application.Interfaces.Appointments;
using BookingSystem.Application.Interfaces.Auth;
using BookingSystem.Application.Interfaces.BlockedHours;
using BookingSystem.Application.Interfaces.Email;
using BookingSystem.Application.Interfaces.Notification;
using BookingSystem.Application.Interfaces.Provider;
using BookingSystem.Application.Interfaces.RedisCache;
using BookingSystem.Application.Interfaces.WorkingHours;
using BookingSystem.Infrastructure.BackgroundServices;
using BookingSystem.Infrastructure.Security;
using BookingSystem.Infrastructure.Services.Appointments;
using BookingSystem.Infrastructure.Services.Auth;
using BookingSystem.Infrastructure.Services.BlockedHours;
using BookingSystem.Infrastructure.Services.Email;
using BookingSystem.Infrastructure.Services.Notifications;
using BookingSystem.Infrastructure.Services.Provider;
using BookingSystem.Infrastructure.Services.RedisCache;
using BookingSystem.Infrastructure.Services.WorkingHour;
using BookingSystem.Infrastructure.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System.Text;

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

        //smtp register
        services.Configure<SmtpSettings>(configuration.GetSection("SmtpSettings"));


        services.AddScoped<ITokenGenerator, TokenGenerator>();

        services.AddScoped<IPasswordHasher, PasswordHasher>();

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IProviderService, ProviderService>();
        services.AddScoped<IWorkingHoursService, WorkingHoursService>();
        services.AddScoped<IBlockedTimeService, BlockedTimeService>();
        services.AddScoped<IAppointmentService, AppointmentService>();

        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<INotificationService, NotificationService>();

        //background worker
        services.AddHostedService<AppointmentCleanupService>();

        services.AddScoped<ICacheService, CacheService>();

        return services;
    }
}
