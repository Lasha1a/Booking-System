using BookingSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingSystem.Persistence.Specifications.BlockedHours;

public class BlockedTimeOverlapSpec : BaseSpecification<BlockedTime>
{
    public BlockedTimeOverlapSpec(Guid providerId, DateOnly startDate, DateOnly endDate)
        : base(bt => bt.ProviderId == providerId
            && bt.StartDate <= endDate
            && bt.EndDate >= startDate)
    {
    }
}
