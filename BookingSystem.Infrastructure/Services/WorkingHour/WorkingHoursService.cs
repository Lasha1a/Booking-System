using BookingSystem.Application.DTOs.WorkingHours;
using BookingSystem.Application.Interfaces.GenericRepo;
using BookingSystem.Application.Interfaces.RedisCache;
using BookingSystem.Application.Interfaces.WorkingHours;
using BookingSystem.Domain.Entities;
using BookingSystem.Persistence.Specifications.WorkingHour;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingSystem.Infrastructure.Services.WorkingHour;

public class WorkingHoursService : IWorkingHoursService
{
    private readonly IGenericRepository<WorkingHours> _repository;
    private readonly ICacheService _cacheService;

    public WorkingHoursService(IGenericRepository<WorkingHours> repository, ICacheService cacheService)
    {
        _repository = repository;
        _cacheService = cacheService;
    }

    public async Task AddWorkingHoursAsync(Guid providerId, AddWorkingHoursRequest request)
    {
        var existing = await _repository
            .GetQueryWithSpec(new WorkingHoursByDaySpec(providerId, request.DayOfWeek))
            .FirstOrDefaultAsync();

        if (existing != null)
            throw new InvalidOperationException("Working hours already exist for this day");

        var workingHours = WorkingHours.Create(
            providerId,
            request.DayOfWeek,
            request.StartTime,
            request.EndTime,
            request.IsActive
        );

        await _repository.AddAsync(workingHours);
        await _repository.SaveChangesAsync();

        // Invalidate cache
        await _cacheService.RemoveAsync($"provider:{providerId}:working-hours");
    }

    public async Task<IReadOnlyList<WorkingHoursResponse>> GetWorkingHoursAsync(Guid providerId)
    {
        var cacheKey = $"provider:{providerId}:working-hours";

        var cached = await _cacheService.GetAsync<List<WorkingHoursResponse>>(cacheKey);
        if (cached != null) return cached;

        var workingHours = await _repository
            .GetQueryWithSpec(new WorkingHoursByProviderSpec(providerId))
            .OrderBy(wh => wh.DayOfWeek)
            .Select(wh => new WorkingHoursResponse
            {
                Id = wh.Id,
                DayOfWeek = wh.DayOfWeek,
                StartTime = wh.StartTime,
                EndTime = wh.EndTime,
                IsActive = wh.IsActive
            })
            .ToListAsync();

        await _cacheService.SetAsync(cacheKey, workingHours, TimeSpan.FromMinutes(10));

        return workingHours;
    }

    public async Task UpdateWorkingHoursAsync(Guid providerId, Guid workingHoursId, UpdateWorkingHoursRequest request)
    {
        var workingHours = await _repository
            .GetQueryWithSpec(new WorkingHoursByIdAndProviderSpec(workingHoursId, providerId))
            .AsTracking()
            .FirstOrDefaultAsync();

        if (workingHours == null)
            throw new KeyNotFoundException("Working hours not found");

        workingHours.UpdateSchedule(request.StartTime, request.EndTime);

        _repository.Update(workingHours);
        await _repository.SaveChangesAsync();

        await _cacheService.RemoveAsync($"provider:{providerId}:working-hours");
    }

    public async Task DeleteWorkingHoursAsync(Guid providerId, Guid workingHoursId)
    {
        var workingHours = await _repository
            .GetQueryWithSpec(new WorkingHoursByIdAndProviderSpec(workingHoursId, providerId))
            .AsTracking()
            .FirstOrDefaultAsync();

        if (workingHours == null)
            throw new KeyNotFoundException("Working hours not found");

        _repository.Delete(workingHours);
        await _repository.SaveChangesAsync();

        await _cacheService.RemoveAsync($"provider:{providerId}:working-hours");
    }
}
