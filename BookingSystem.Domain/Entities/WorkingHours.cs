using BookingSystem.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingSystem.Domain.Entities;

public class WorkingHours : BaseEntity
{
    public Guid ProviderId { get; private set; }
    public DayOfWeek DayOfWeek { get; private set; }
    public TimeOnly StartTime { get; private set; }
    public TimeOnly EndTime { get; private set; }
    public bool IsActive { get; private set; } = false;

    public ServiceProvider Provider { get; private set; } = null!;

    private WorkingHours() { }

    public static WorkingHours Create(Guid providerId, DayOfWeek dayOfWeek, TimeOnly startTime, TimeOnly endTime, bool isActive)
    {
        return new WorkingHours()
        {
            ProviderId = providerId,
            DayOfWeek = dayOfWeek,
            StartTime = startTime,
            EndTime = endTime,
            IsActive = isActive
        };
    }

    public void UpdateSchedule(TimeOnly startTime, TimeOnly endTime)
    {
        StartTime = startTime;
        EndTime = endTime;
    }

    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;

}
