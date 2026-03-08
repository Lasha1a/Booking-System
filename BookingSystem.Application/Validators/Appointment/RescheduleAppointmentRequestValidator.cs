using BookingSystem.Application.DTOs.Appointment;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingSystem.Application.Validators.Appointment;

public class RescheduleAppointmentRequestValidator : AbstractValidator<RescheduleAppointmentRequest>
{
    public RescheduleAppointmentRequestValidator()
    {
        RuleFor(x => x.NewDate)
            .NotEmpty().WithMessage("New date is required")
            .GreaterThan(DateTime.UtcNow).WithMessage("New date must be in the future");

        RuleFor(x => x.NewEndTime)
            .GreaterThan(x => x.NewStartTime).WithMessage("End time must be after start time");
    }
}
