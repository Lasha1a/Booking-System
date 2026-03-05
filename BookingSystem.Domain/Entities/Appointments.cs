using BookingSystem.Domain.Common;
using BookingSystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingSystem.Domain.Entities;

public class Appointment : BaseEntity
{
    public Guid ProviderId { get; private set; }
    public string CustomerName { get; private set; } = string.Empty;
    public string CustomerEmail { get; private set; } = string.Empty;
    public string CustomerPhone { get; private set; } = string.Empty;
    public DateTime AppointmentDate { get; private set; }
    public TimeOnly StartTime { get; private set; }
    public TimeOnly EndTime { get; private set; }
    public AppointmentStatus Status { get; private set; } = AppointmentStatus.Scheduled;
    public string CancellationReason { get; private set; } = string.Empty;
    public bool IsRecurring { get; private set; } = false;
    public string ReccurenceRule { get; private set; } = string.Empty;
    public Guid? ParentAppointmentId { get; private set; } // For recurring appointments, reference to the parent appointment
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; private set; } = DateTime.UtcNow;
    //navigation properties
    public ServiceProvider Provider { get; private set; } = null!;

    public Appointment? ParentAppointment { get; private set; } // Navigation property for parent appointment
    public ICollection<Appointment> ChildAppointments { get; private set; } = new List<Appointment>();

    public ICollection<NotificationLog> NotificationLogs { get; private set; } = new List<NotificationLog>();


    private Appointment() { }

    //static factory method to create appointment
    public static Appointment Create(
        Guid providerId,
        string customerName,
        string customerEmail,
        string customerPhone,
        DateTime appointmentDate,
        TimeOnly startTime,
        TimeOnly endTime,
        bool isRecurring = false,
        string reccurenceRule = "",
        Guid? parentAppointmentId = null)
    {
        return new Appointment
        {
            ProviderId = providerId,
            CustomerName = customerName,
            CustomerEmail = customerEmail,
            CustomerPhone = customerPhone,
            AppointmentDate = appointmentDate,
            StartTime = startTime,
            EndTime = endTime,
            Status = AppointmentStatus.Scheduled,
            IsRecurring = isRecurring,
            ReccurenceRule = reccurenceRule,
            ParentAppointmentId = parentAppointmentId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    //business methods
    public void Cancel(string reason)
    {
        if (Status == AppointmentStatus.Completed)
            throw new InvalidOperationException("cannot cancel completed appointment");

        if (Status == AppointmentStatus.Cancelled)
            throw new InvalidOperationException("Appointment is already cancelled");

        Status = AppointmentStatus.Cancelled;
        CancellationReason = reason;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Complete()
    {
        if (Status == AppointmentStatus.Cancelled)
            throw new InvalidOperationException("Cannot complete a cancelled appointment");

        Status = AppointmentStatus.Completed;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkAsNoShow()
    {
        if (Status != AppointmentStatus.Scheduled)
            throw new InvalidOperationException("Only scheduled appointments can be marked as no show");

        Status = AppointmentStatus.NoShow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Reschedule(DateTime newDate, TimeOnly newStartTime, TimeOnly newEndTime)
    {
        if (Status == AppointmentStatus.Cancelled)
            throw new InvalidOperationException("Cannot reschedule a cancelled appointment");

        if (Status == AppointmentStatus.Completed)
            throw new InvalidOperationException("Cannot reschedule a completed appointment");

        AppointmentDate = newDate;
        StartTime = newStartTime;
        EndTime = newEndTime;
        UpdatedAt = DateTime.UtcNow;
    }
}
