using BookingSystem.Application;
using BookingSystem.Infrastructure;
using BookingSystem.Persistence;

namespace BookingSystem;

public static class DependencyInjection
{
    public static IServiceCollection AddMainApiDI(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddInfrastructureDI(configuration)
            .AddApplicationDI()
            .AddPersistanceDI(configuration);


        return services;
    }
}
