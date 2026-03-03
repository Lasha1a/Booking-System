using BookingSystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingSystem.Domain.Entities;

public class NotificationLog
{
    public Guid Id { get; set; }
    public Guid AppointmentId { get; set; }
    public NotificationType NotificationType { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime SentAt { get; set; } = DateTime.UtcNow;

    public Appointment Appointment { get; set; } = null!;
}
