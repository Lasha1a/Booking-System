using BookingSystem.Application.DTOs.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingSystem.Application.Interfaces.Auth;

public interface IAuthService
{
    Task RegisterAsync(RegisterUserRequest request);
    Task<AuthResponse> LoginAsync(LoginRequest request);
}
