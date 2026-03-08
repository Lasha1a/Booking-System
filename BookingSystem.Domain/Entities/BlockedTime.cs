using BookingSystem.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingSystem.Domain.Entities;

public class BlockedTime : BaseEntity
{
    public Guid ProviderId { get; private set; }
    public DateOnly StartDate { get; private set; }
    public DateOnly EndDate { get; private set; }
    public string Reason { get; private set; } = string.Empty;
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    public ServiceProvider Provider { get; private set; } = null!;

    private BlockedTime() { }

    public static BlockedTime Create(Guid providedId, DateOnly startDate, DateOnly endDate, string reason)
    {
        return new BlockedTime
        {
            ProviderId = providedId,
            StartDate = startDate,
            EndDate = endDate,
            Reason = reason, 
            CreatedAt = DateTime.UtcNow

        };
    }

    public void UpdateDates(DateOnly startDate, DateOnly endDate, string reason)
    {
        StartDate = startDate;
        EndDate = endDate;
        Reason = reason;
    }
}
