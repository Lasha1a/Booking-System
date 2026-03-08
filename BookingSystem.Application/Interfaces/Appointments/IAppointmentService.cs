using BookingSystem.Application.DTOs.Appointment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingSystem.Application.Interfaces.Appointments;

public interface IAppointmentService
{
    // Customer side
    Task<List<AvailableSlotResponse>> GetAvailableSlotsAsync(AvailableSlotsRequest request);
    Task<AppointmentResponse> BookAppointmentAsync(BookAppointmentRequest request);
    Task CancelAppointmentAsync(Guid appointmentId, CancelAppointmentRequest request);
    Task RescheduleAppointmentAsync(Guid appointmentId, RescheduleAppointmentRequest request);

    // Provider side
    Task<IReadOnlyList<AppointmentResponse>> GetProviderAppointmentsAsync(Guid providerId);
    Task CompleteAppointmentAsync(Guid providerId, Guid appointmentId);
    Task MarkAsNoShowAsync(Guid providerId, Guid appointmentId);
}
