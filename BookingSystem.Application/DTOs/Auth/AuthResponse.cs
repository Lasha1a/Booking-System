using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingSystem.Application.DTOs.Auth;

public class AuthResponse
{
    public string Token { get; set; } = string.Empty;
}
