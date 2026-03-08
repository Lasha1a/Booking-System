using BookingSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingSystem.Persistence.Specifications.WorkingHour;

public class WorkingHoursByDaySpec : BaseSpecification<WorkingHours>
{
    public WorkingHoursByDaySpec(Guid providerId, DayOfWeek dayOfWeek)
        : base(wh => wh.ProviderId == providerId && wh.DayOfWeek == dayOfWeek)
    {
    }
}
