using BookingSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingSystem.Application.Interfaces.Notification;

public interface INotificationService
{
    Task SendBookingConfirmationAsync(Appointment appointment);
    Task SendCancellationNotificationAsync(Appointment appointment);
    Task SendReminderAsync(Appointment appointment);
}
