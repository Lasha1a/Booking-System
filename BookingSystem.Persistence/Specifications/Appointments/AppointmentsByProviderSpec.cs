using BookingSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingSystem.Persistence.Specifications.Appointments;

public class AppointmentsByProviderSpec : BaseSpecification<Appointment>
{
    public AppointmentsByProviderSpec(Guid providerId)
        : base(a => a.ProviderId == providerId)
    {
    }
}
