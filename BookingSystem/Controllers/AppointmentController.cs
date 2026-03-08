using BookingSystem.Application.DTOs.Appointment;
using BookingSystem.Application.Interfaces.Appointments;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BookingSystem.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AppointmentController : BaseController
{
    private readonly IAppointmentService _appointmentService;

    public AppointmentController(IAppointmentService appointmentService)
    {
        _appointmentService = appointmentService;
    }

    // Customer side - no auth needed
    [HttpGet("available-slots")]
    public async Task<IActionResult> GetAvailableSlots([FromQuery] AvailableSlotsRequest request)
    {
        var slots = await _appointmentService.GetAvailableSlotsAsync(request);
        return Ok(slots);
    }

    [HttpPost("booking")]
    public async Task<IActionResult> BookAppointment([FromBody] BookAppointmentRequest request)
    {
        var appointment = await _appointmentService.BookAppointmentAsync(request);
        return Ok(appointment);
    }

    [HttpPut("{id}/cancel")]
    public async Task<IActionResult> CancelAppointment(Guid id, [FromBody] CancelAppointmentRequest request)
    {
        await _appointmentService.CancelAppointmentAsync(id, request);
        return Ok("Appointment cancelled successfully");
    }

    [HttpPut("{id}/reschedule")]
    public async Task<IActionResult> RescheduleAppointment(Guid id, [FromBody] RescheduleAppointmentRequest request)
    {
        await _appointmentService.RescheduleAppointmentAsync(id, request);
        return Ok("Appointment rescheduled successfully");
    }

    // Provider side - auth needed
    [HttpGet("my-appointments")]
    [Authorize]
    public async Task<IActionResult> GetProviderAppointments()
    {
        var providerId = GetProviderIdFromToken();
        var appointments = await _appointmentService.GetProviderAppointmentsAsync(providerId);
        return Ok(appointments);
    }

    [HttpPut("{id}/complete")]
    [Authorize]
    public async Task<IActionResult> CompleteAppointment(Guid id)
    {
        var providerId = GetProviderIdFromToken();
        await _appointmentService.CompleteAppointmentAsync(providerId, id);
        return Ok("Appointment completed successfully");
    }

    [HttpPut("{id}/no-show")]
    [Authorize]
    public async Task<IActionResult> MarkAsNoShow(Guid id)
    {
        var providerId = GetProviderIdFromToken();
        await _appointmentService.MarkAsNoShowAsync(providerId, id);
        return Ok("Appointment marked as no show");
    }
}
