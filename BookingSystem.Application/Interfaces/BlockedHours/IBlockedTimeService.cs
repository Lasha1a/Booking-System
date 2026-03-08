using BookingSystem.Application.DTOs.BlockedTimes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingSystem.Application.Interfaces.BlockedHours;

public interface IBlockedTimeService
{
    Task AddBlockedTimeAsync(Guid providerId, AddBlockedTimeRequest request);
    Task<IReadOnlyList<BlockedTimeResponse>> GetBlockedTimesAsync(Guid providerId);
    Task UpdateBlockedTimeAsync(Guid providerId, Guid blockedTimeId, UpdateBlockedTimeRequest request);
    Task DeleteBlockedTimeAsync(Guid providerId, Guid blockedTimeId);
}
