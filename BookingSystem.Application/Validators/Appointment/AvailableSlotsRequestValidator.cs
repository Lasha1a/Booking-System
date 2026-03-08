using BookingSystem.Application.DTOs.Appointment;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingSystem.Application.Validators.Appointment;

public class AvailableSlotsRequestValidator : AbstractValidator<AvailableSlotsRequest>
{
    public AvailableSlotsRequestValidator()
    {
        RuleFor(x => x.ProviderId)
            .NotEmpty().WithMessage("Provider is required");

        RuleFor(x => x.Date)
            .NotEmpty().WithMessage("Date is required");

        RuleFor(x => x.DurationMinutes)
            .NotEmpty().WithMessage("Duration is required")
            .Must(x => x == 15 || x == 30 || x == 45 || x == 60)
            .WithMessage("Duration must be 15, 30, 45 or 60 minutes");
    }
}
