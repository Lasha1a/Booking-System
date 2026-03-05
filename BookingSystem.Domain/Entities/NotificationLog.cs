using BookingSystem.Domain.Common;
using BookingSystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingSystem.Domain.Entities;

public class NotificationLog : BaseEntity
{
    public Guid AppointmentId { get; private set; }
    public NotificationType NotificationType { get; private set; }
    public NotificationStatus Status { get; private set; } = NotificationStatus.Pending;
    public DateTime SentAt { get; private set; } = DateTime.UtcNow;

    public Appointment Appointment { get; private set; } = null!;

    public static NotificationLog Create(Guid appointmentId, NotificationType notificationType)
    {
        return new NotificationLog
        {
            AppointmentId = appointmentId,
            NotificationType = notificationType,
            SentAt = DateTime.UtcNow,
            Status = NotificationStatus.Pending  // starts as pending
        };
    }

    public void MarkAsSent() => Status = NotificationStatus.Sent;
    public void MarkAsFailed() => Status = NotificationStatus.Failed;
}
