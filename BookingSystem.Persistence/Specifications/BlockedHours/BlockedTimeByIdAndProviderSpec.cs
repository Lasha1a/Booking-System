using BookingSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingSystem.Persistence.Specifications.BlockedHours;

public class BlockedTimeByIdAndProviderSpec : BaseSpecification<BlockedTime>
{
    public BlockedTimeByIdAndProviderSpec(Guid blockedTimeId, Guid providerId)
       : base(bt => bt.Id == blockedTimeId && bt.ProviderId == providerId)
    {
    }
}
