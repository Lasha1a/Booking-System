using BookingSystem.Application.DTOs.Provider;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingSystem.Application.Validators.Providers;

public class UpdateProviderRequestValidator : AbstractValidator<UpdateProviderRequest>
{
    public UpdateProviderRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is Required")
            .MaximumLength(100).WithMessage("name cannot exceed 100 characters");

        RuleFor(x => x.Specialty)
             .NotEmpty().WithMessage("Specialty is required")
             .MaximumLength(100).WithMessage("Specialty cannot exceed 100 characters");
    }
}
