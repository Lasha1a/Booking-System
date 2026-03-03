using BookingSystem.Application;
using BookingSystem.Infrastructure;
using BookingSystem.Persistence;

namespace BookingSystem;

public static class DependencyInjection
{
    public static IServiceCollection AddMainApiDI(this IServiceCollection services)
    {
        services.AddInfrastructureDI()
            .AddApplicationDI()
            .AddPersistanceDI();


        return services;
    }
}
