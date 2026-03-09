using BookingSystem.Application.Interfaces.Email;
using BookingSystem.Application.Interfaces.GenericRepo;
using BookingSystem.Application.Interfaces.Notification;
using BookingSystem.Domain.Entities;
using BookingSystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingSystem.Infrastructure.Services.Notifications;

public class NotificationService : INotificationService
{
    private readonly IEmailService _emailService;
    private readonly IGenericRepository<NotificationLog> _notificationRepository;

    public NotificationService(
        IEmailService emailService,
        IGenericRepository<NotificationLog> notificationRepository)
    {
        _emailService = emailService;
        _notificationRepository = notificationRepository;
    }

    public async Task SendBookingConfirmationAsync(Appointment appointment)
    {
        var subject = "Appointment Confirmation";
        var body = $@"
            <h2>Appointment Confirmed!</h2>
            <p>Dear {appointment.CustomerName},</p>
            <p>Your appointment has been confirmed.</p>
            <p><strong>Date:</strong> {appointment.AppointmentDate:MMMM dd, yyyy}</p>
            <p><strong>Time:</strong> {appointment.StartTime} - {appointment.EndTime}</p>
            <p>Thank you for booking with us!</p>
        ";

        await SendNotificationAsync(appointment, NotificationType.Confirmation, subject, body);
    }

    public async Task SendCancellationNotificationAsync(Appointment appointment)
    {
        var subject = "Appointment Cancelled";
        var body = $@"
            <h2>Appointment Cancelled</h2>
            <p>Dear {appointment.CustomerName},</p>
            <p>Your appointment has been cancelled.</p>
            <p><strong>Date:</strong> {appointment.AppointmentDate:MMMM dd, yyyy}</p>
            <p><strong>Time:</strong> {appointment.StartTime} - {appointment.EndTime}</p>
            <p><strong>Reason:</strong> {appointment.CancellationReason}</p>
        ";

        await SendNotificationAsync(appointment, NotificationType.Cancellation, subject, body);
    }

    public async Task SendReminderAsync(Appointment appointment)
    {
        var subject = "Appointment Reminder";
        var body = $@"
            <h2>Appointment Reminder</h2>
            <p>Dear {appointment.CustomerName},</p>
            <p>This is a reminder that you have an appointment tomorrow.</p>
            <p><strong>Date:</strong> {appointment.AppointmentDate:MMMM dd, yyyy}</p>
            <p><strong>Time:</strong> {appointment.StartTime} - {appointment.EndTime}</p>
            <p>See you soon!</p>
        ";

        await SendNotificationAsync(appointment, NotificationType.Reminder, subject, body);
    }

    //helper
    private async Task SendNotificationAsync(Appointment appointment,  NotificationType type, string subject, string body)
    {
        var log = NotificationLog.Create(appointment.Id, type);

        try
        {
            await _emailService.SendEmailAsync(
               appointment.CustomerEmail,
               appointment.CustomerName,
               subject,
               body);

            log.MarkAsSent();
        }
        catch(Exception)
        {
            log.MarkAsFailed();
        }
        finally
        {
            await _notificationRepository.AddAsync(log);
            await _notificationRepository.SaveChangesAsync();
        }
    }
}
