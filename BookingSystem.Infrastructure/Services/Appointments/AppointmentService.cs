using BookingSystem.Application.DTOs.Appointment;
using BookingSystem.Application.Interfaces.Appointments;
using BookingSystem.Application.Interfaces.GenericRepo;
using BookingSystem.Application.Interfaces.Notification;
using BookingSystem.Application.Interfaces.RedisCache;
using BookingSystem.Domain.Entities;
using BookingSystem.Persistence.Specifications.Appointments;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingSystem.Infrastructure.Services.Appointments;

public class AppointmentService : IAppointmentService
{
    private readonly IGenericRepository<Appointment> _appointmentRepository;
    private readonly IGenericRepository<WorkingHours> _workingHoursRepository;
    private readonly IGenericRepository<BlockedTime> _blockedTimeRepository;
    private readonly INotificationService _notificationService;
    private readonly ICacheService _cacheService;
    public AppointmentService(
        IGenericRepository<Appointment> appointmentRepository,
        IGenericRepository<WorkingHours> workingHoursRepository,
        IGenericRepository<BlockedTime> blockedTimeRepository,
        INotificationService notificationService,
        ICacheService cacheService)
    {
        _appointmentRepository = appointmentRepository;
        _workingHoursRepository = workingHoursRepository;
        _blockedTimeRepository = blockedTimeRepository;
        _notificationService = notificationService;
        _cacheService = cacheService;
    }

    public async Task<List<AvailableSlotResponse>> GetAvailableSlotsAsync(AvailableSlotsRequest request)
    {
        var cacheKey = $"provider:{request.ProviderId}:slots:{request.Date}:{request.DurationMinutes}";

        // Check cache first
        var cached = await _cacheService.GetAsync<List<AvailableSlotResponse>>(cacheKey);
        if (cached != null) return cached;

        var dayOfWeek = request.Date.DayOfWeek;

        //Check if provider works that day
        var workingHours = await _workingHoursRepository.GetAll()
            .Where(wh => wh.ProviderId == request.ProviderId
                && wh.DayOfWeek == dayOfWeek
                && wh.IsActive)
            .FirstOrDefaultAsync();

        if (workingHours == null)
            return new List<AvailableSlotResponse>();

        //Check if provider is blocked that day
        var isBlocked = await _blockedTimeRepository.GetAll()
            .AnyAsync(bt => bt.ProviderId == request.ProviderId
                && bt.StartDate <= request.Date
                && bt.EndDate >= request.Date);

        if (isBlocked)
            return new List<AvailableSlotResponse>();

        // Get all booked appointments for that day
        var bookedAppointments = await _appointmentRepository
            .GetQueryWithSpec(new BookedAppointmentsByDateSpec(request.ProviderId, request.Date))
            .Select(a => new { a.StartTime, a.EndTime })
            .ToListAsync();

        //Generate available slots
        var availableSlots = new List<AvailableSlotResponse>();
        var slotStart = workingHours.StartTime;
        var duration = TimeSpan.FromMinutes(request.DurationMinutes);

        while (slotStart.ToTimeSpan().Add(duration) <= workingHours.EndTime.ToTimeSpan())
        {
            var slotEnd = slotStart.Add(duration);

            // Check if slot overlaps with any booked appointment
            var isBooked = bookedAppointments.Any(a =>
                a.StartTime < slotEnd && a.EndTime > slotStart);

            if (!isBooked)
            {
                availableSlots.Add(new AvailableSlotResponse
                {
                    StartTime = slotStart,
                    EndTime = slotEnd
                });
            }

            slotStart = slotEnd;
        }

        await _cacheService.SetAsync(cacheKey, availableSlots, TimeSpan.FromMinutes(5));

        return availableSlots;
    }

    public async Task<AppointmentResponse> BookAppointmentAsync(BookAppointmentRequest request)
    {
        // Check for conflicts
        var conflict = await _appointmentRepository
            .GetQueryWithSpec(new AppointmentConflictSpec(
                request.ProviderId,
                request.AppointmentDate,
                request.StartTime,
                request.EndTime))
            .FirstOrDefaultAsync();

        if (conflict != null)
            throw new InvalidOperationException("This time slot is already booked");

        //Create appointment
        var appointment = Appointment.Create(
            request.ProviderId,
            request.CustomerName,
            request.CustomerEmail,
            request.CustomerPhone,
            request.AppointmentDate,
            request.StartTime,
            request.EndTime,
            request.IsRecurring,
            request.RecurrenceRule
        );

        await _appointmentRepository.AddAsync(appointment);

        // Handle recurring appointments
        if (request.IsRecurring && !string.IsNullOrEmpty(request.RecurrenceRule))
        {
            await CreateRecurringAppointmentsAsync(appointment, request);
        }

        await _appointmentRepository.SaveChangesAsync();

        // Invalidate cache
        await _cacheService.RemoveAsync($"provider:{request.ProviderId}:appointments");
        await _cacheService.RemoveAsync($"provider:{request.ProviderId}:slots:{DateOnly.FromDateTime(request.AppointmentDate)}:{request.EndTime - request.StartTime}");

        //added email sender on booking endpoint for confirmation
        await _notificationService.SendBookingConfirmationAsync(appointment);

        return MapToResponse(appointment);
    }

    public async Task CancelAppointmentAsync(Guid appointmentId, CancelAppointmentRequest request)
    {
        var appointment = await _appointmentRepository
            .GetQueryWithSpec(new AppointmentByIdSpec(appointmentId))
            .AsTracking()
            .FirstOrDefaultAsync();

        if (appointment == null)
            throw new KeyNotFoundException("Appointment not found");

        appointment.Cancel(request.Reason);

        // If recurring cancel all children too
        if (appointment.IsRecurring && appointment.ParentAppointmentId == null)
        {
            var children = await _appointmentRepository
                .GetQueryWithSpec(new RecurringChildAppointmentsSpec(appointmentId))
                .ToListAsync();

            foreach (var child in children)
                child.Cancel(request.Reason);
        }

        _appointmentRepository.Update(appointment);
        await _appointmentRepository.SaveChangesAsync();

        await _cacheService.RemoveAsync($"provider:{appointment.ProviderId}:appointments");

        await _notificationService.SendCancellationNotificationAsync(appointment);

        
    }

    public async Task RescheduleAppointmentAsync(Guid appointmentId, RescheduleAppointmentRequest request)
    {
        var appointment = await _appointmentRepository
            .GetQueryWithSpec(new AppointmentByIdSpec(appointmentId))
            .AsTracking()
            .FirstOrDefaultAsync();

        if (appointment == null)
            throw new KeyNotFoundException("Appointment not found");

        // Check for conflicts on new date
        var conflict = await _appointmentRepository
            .GetQueryWithSpec(new AppointmentConflictSpec(
                appointment.ProviderId,
                request.NewDate,
                request.NewStartTime,
                request.NewEndTime))
            .Where(a => a.Id != appointmentId)
            .FirstOrDefaultAsync();

        if (conflict != null)
            throw new InvalidOperationException("This time slot is already booked");

        appointment.Reschedule(request.NewDate, request.NewStartTime, request.NewEndTime);

        _appointmentRepository.Update(appointment);
        await _appointmentRepository.SaveChangesAsync();

        await _cacheService.RemoveAsync($"provider:{appointment.ProviderId}:appointments");
    }

    //provider side
    public async Task<IReadOnlyList<AppointmentResponse>> GetProviderAppointmentsAsync(Guid providerId)
    {
        var cacheKey = $"provider:{providerId}:appointments";

        // 1. Check cache first
        var cached = await _cacheService.GetAsync<List<AppointmentResponse>>(cacheKey);
        if (cached != null) return cached;

        var appointments = await _appointmentRepository
            .GetQueryWithSpec(new AppointmentsByProviderSpec(providerId))
            .OrderBy(a => a.AppointmentDate)
            .ThenBy(a => a.StartTime)
            .ToListAsync();

        var response = appointments.Select(a => MapToResponseExpression(a)).ToList();

        await _cacheService.SetAsync(cacheKey, response, TimeSpan.FromMinutes(5));


        return response;
    }

    public async Task CompleteAppointmentAsync(Guid providerId, Guid appointmentId)
    {
        var appointment = await _appointmentRepository
            .GetQueryWithSpec(new AppointmentByIdSpec(appointmentId))
            .Where(a => a.ProviderId == providerId)
            .AsTracking() //ef core tracks the updates with this
            .FirstOrDefaultAsync();

        if (appointment == null)
            throw new KeyNotFoundException("Appointment not found");

        appointment.Complete();

        _appointmentRepository.Update(appointment);
        await _appointmentRepository.SaveChangesAsync();

        await _cacheService.RemoveAsync($"provider:{providerId}:appointments");
    }

    public async Task MarkAsNoShowAsync(Guid providerId, Guid appointmentId)
    {
        var appointment = await _appointmentRepository
            .GetQueryWithSpec(new AppointmentByIdSpec(appointmentId))
            .Where(a => a.ProviderId == providerId)
            .AsTracking()
            .FirstOrDefaultAsync();

        if (appointment == null)
            throw new KeyNotFoundException("Appointment not found");

        appointment.MarkAsNoShow();

        _appointmentRepository.Update(appointment);
        await _appointmentRepository.SaveChangesAsync();

        await _cacheService.RemoveAsync($"provider:{providerId}:appointments");
    }



    //private helpers
    private async Task CreateRecurringAppointmentsAsync(Appointment parent, BookAppointmentRequest request)
    {
        var dates = new List<DateTime>();

        if (request.RecurrenceRule == "weekly")
        {
            for (int i = 1; i <= 4; i++)
                dates.Add(request.AppointmentDate.AddDays(7 * i));
        }
        else if (request.RecurrenceRule == "monthly")
        {
            for (int i = 1; i <= 3; i++)
                dates.Add(request.AppointmentDate.AddMonths(i));
        }

        foreach (var date in dates)
        {
            var child = Appointment.Create(
                request.ProviderId,
                request.CustomerName,
                request.CustomerEmail,
                request.CustomerPhone,
                date,
                request.StartTime,
                request.EndTime,
                true,
                request.RecurrenceRule,
                parent.Id
            );

            await _appointmentRepository.AddAsync(child);
        }
    }

    private AppointmentResponse MapToResponse(Appointment appointment)
    {
        return new AppointmentResponse
        {
            Id = appointment.Id,
            ProviderId = appointment.ProviderId,
            CustomerName = appointment.CustomerName,
            CustomerEmail = appointment.CustomerEmail,
            CustomerPhone = appointment.CustomerPhone,
            AppointmentDate = appointment.AppointmentDate,
            StartTime = appointment.StartTime,
            EndTime = appointment.EndTime,
            Status = appointment.Status,
            IsRecurring = appointment.IsRecurring,
            RecurrenceRule = appointment.ReccurenceRule,
            CreatedAt = appointment.CreatedAt
        };
    }

    private static AppointmentResponse MapToResponseExpression(Appointment a)
    {
        return new AppointmentResponse
        {
            Id = a.Id,
            ProviderId = a.ProviderId,
            CustomerName = a.CustomerName,
            CustomerEmail = a.CustomerEmail,
            CustomerPhone = a.CustomerPhone,
            AppointmentDate = a.AppointmentDate,
            StartTime = a.StartTime,
            EndTime = a.EndTime,
            Status = a.Status,
            IsRecurring = a.IsRecurring,
            RecurrenceRule = a.ReccurenceRule,
            CreatedAt = a.CreatedAt
        };
    }
}
