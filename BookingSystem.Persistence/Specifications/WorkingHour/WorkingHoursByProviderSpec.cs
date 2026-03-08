using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookingSystem.Domain.Entities;
namespace BookingSystem.Persistence.Specifications.WorkingHour;

public class WorkingHoursByProviderSpec : BaseSpecification<WorkingHours>
{
    public WorkingHoursByProviderSpec(Guid providerId)
       : base(wh => wh.ProviderId == providerId)
    {
    }
}
