using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingSystem.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistanceDI(this IServiceCollection services)
    {
        return services;
    }
}
