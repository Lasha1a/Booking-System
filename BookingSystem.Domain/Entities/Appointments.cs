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
    public Guid ProviderId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public string CustomerPhone { get; set; } = string.Empty;
    public DateTime AppointmentDate { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public AppointmentStatus Status { get; set; } = AppointmentStatus.Scheduled;
    public string CancellationReason { get; set; } = string.Empty;
    public bool IsRecurring { get; set; } = false;
    public string ReccurenceRule { get; set; } = string.Empty;
    public Guid? ParentAppointmentId { get; set; } // For recurring appointments, reference to the parent appointment
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    //navigation properties
    public ServiceProvider Provider { get; set; } = null!;

    public Appointment? ParentAppointment { get; set; } // Navigation property for parent appointment
    public ICollection<Appointment> ChildAppointments { get; set; } = new List<Appointment>();

    public ICollection<NotificationLog> NotificationLogs { get; set; } = new List<NotificationLog>();
}
