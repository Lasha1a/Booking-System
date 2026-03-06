using BookingSystem.Application.DTOs.Auth;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingSystem.Application.Validators.Auth;

public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is Required")
            .EmailAddress().WithMessage("invalid email format");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("password is required");
    }
}
