using BookingSystem.Application.DTOs.WorkingHours;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingSystem.Application.Validators.WorkingHours;

public class AddWorkingHoursRequestValidator : AbstractValidator<AddWorkingHoursRequest>
{
    public AddWorkingHoursRequestValidator()
    {
        RuleFor(x => x.DayOfWeek)
            .IsInEnum().WithMessage("Invalid day of week");

        RuleFor(x => x.StartTime)
            .NotEmpty().WithMessage("Start time is required");

        RuleFor(x => x.EndTime)
            .NotEmpty().WithMessage("End time is required")
            .GreaterThan(x => x.StartTime).WithMessage("End time must be after start time");
    }
}
