using BookingSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingSystem.Persistence.Specifications.BlockedHours;

public class BlockedTimesByProviderSpec : BaseSpecification<BlockedTime>
{
    public BlockedTimesByProviderSpec(Guid providerId)
        : base(bt => bt.ProviderId == providerId)
    {
    }
}
