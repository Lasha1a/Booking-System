using BookingSystem.Application.Interfaces.Auth;
using Microsoft.AspNetCore.Http;
using BookingSystem.Application.DTOs.Auth;
using Microsoft.AspNetCore.Mvc;

namespace BookingSystem.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserRequest request)
    {
        await _authService.RegisterAsync(request);
        return Ok("registered succesfully!!");
    }

    [HttpPost("log-in")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var response = await _authService.LoginAsync(request);
        return Ok(response);
    }
}
