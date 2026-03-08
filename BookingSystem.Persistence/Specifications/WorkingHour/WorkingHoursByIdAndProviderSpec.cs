using BookingSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingSystem.Persistence.Specifications.WorkingHour;

public class WorkingHoursByIdAndProviderSpec : BaseSpecification<WorkingHours>
{
    public WorkingHoursByIdAndProviderSpec(Guid workingHoursId, Guid providerId)
        : base(wh => wh.Id == workingHoursId && wh.ProviderId == providerId)
    {
    }
}
