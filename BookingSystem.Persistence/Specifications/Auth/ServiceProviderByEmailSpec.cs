using BookingSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingSystem.Persistence.Specifications.Auth;

public class ServiceProviderByEmailSpec : BaseSpecification<ServiceProvider>
{

    // finds specific serviceprovider for matched email
    public ServiceProviderByEmailSpec(string email)
        : base(sp => sp.Email.ToLower() == email.ToLower()) { }
}
