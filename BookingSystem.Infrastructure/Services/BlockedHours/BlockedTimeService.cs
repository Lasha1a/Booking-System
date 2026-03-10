using BookingSystem.Application.DTOs.BlockedTimes;
using BookingSystem.Application.Interfaces.BlockedHours;
using BookingSystem.Application.Interfaces.GenericRepo;
using BookingSystem.Application.Interfaces.RedisCache;
using BookingSystem.Domain.Entities;
using BookingSystem.Persistence.Specifications.BlockedHours;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingSystem.Infrastructure.Services.BlockedHours;

public class BlockedTimeService : IBlockedTimeService
{
    private readonly IGenericRepository<BlockedTime> _repository;
    private readonly ICacheService _cacheService;

    public BlockedTimeService(IGenericRepository<BlockedTime> repository, ICacheService cacheService)
    {
        _repository = repository;
        _cacheService = cacheService;
    }

    public async Task AddBlockedTimeAsync(Guid providerId, AddBlockedTimeRequest request)
    {
        // Check for overlapping blocked times
        var overlap = await _repository
            .GetQueryWithSpec(new BlockedTimeOverlapSpec(providerId, request.StartDate, request.EndDate))
            .FirstOrDefaultAsync();

        if (overlap != null)
            throw new InvalidOperationException("Blocked time overlaps with an existing blocked time");

        var blockedTime = BlockedTime.Create(
            providerId,
            request.StartDate,
            request.EndDate,
            request.Reason
        );

        await _repository.AddAsync(blockedTime);
        await _repository.SaveChangesAsync();

        await _cacheService.RemoveAsync($"provider:{providerId}:blocked-times");
    }

    public async Task<IReadOnlyList<BlockedTimeResponse>> GetBlockedTimesAsync(Guid providerId)
    {
        var cacheKey = $"provider:{providerId}:blocked-times";

        var cached = await _cacheService.GetAsync<List<BlockedTimeResponse>>(cacheKey);
        if (cached != null) return cached;

        var blockedTimes = await _repository
            .GetQueryWithSpec(new BlockedTimesByProviderSpec(providerId))
            .OrderBy(bt => bt.StartDate)
            .Select(bt => new BlockedTimeResponse
            {
                Id = bt.Id,
                StartDate = bt.StartDate,
                EndDate = bt.EndDate,
                Reason = bt.Reason,
                CreatedAt = bt.CreatedAt
            })
            .ToListAsync();

        await _cacheService.SetAsync(cacheKey, blockedTimes, TimeSpan.FromMinutes(10));

        return blockedTimes;
    }

    public async Task UpdateBlockedTimeAsync(Guid providerId, Guid blockedTimeId, UpdateBlockedTimeRequest request)
    {
        var blockedTime = await _repository
            .GetQueryWithSpec(new BlockedTimeByIdAndProviderSpec(blockedTimeId, providerId))
            .AsTracking()
            .FirstOrDefaultAsync();

        if (blockedTime == null)
            throw new KeyNotFoundException("Blocked time not found");

        // Check for overlapping blocked times excluding current one
        var overlap = await _repository
            .GetQueryWithSpec(new BlockedTimeOverlapSpec(providerId, request.StartDate, request.EndDate))
            .Where(bt => bt.Id != blockedTimeId)
            .FirstOrDefaultAsync();

        if (overlap != null)
            throw new InvalidOperationException("Blocked time overlaps with an existing blocked time");

        blockedTime.UpdateDates(request.StartDate, request.EndDate, request.Reason);

        _repository.Update(blockedTime);
        await _repository.SaveChangesAsync();

        await _cacheService.RemoveAsync($"provider:{providerId}:blocked-times");
    }

    public async Task DeleteBlockedTimeAsync(Guid providerId, Guid blockedTimeId)
    {
        var blockedTime = await _repository
            .GetQueryWithSpec(new BlockedTimeByIdAndProviderSpec(blockedTimeId, providerId))
            .AsTracking()
            .FirstOrDefaultAsync();

        if (blockedTime == null)
            throw new KeyNotFoundException("Blocked time not found");

        _repository.Delete(blockedTime);
        await _repository.SaveChangesAsync();

        await _cacheService.RemoveAsync($"provider:{providerId}:blocked-times");
    }
}
