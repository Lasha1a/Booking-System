using BookingSystem.Application.DTOs.Provider;
using BookingSystem.Application.Interfaces.Provider;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace BookingSystem.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class ProviderController : ControllerBase
{
    private readonly IProviderService _providerService;

    public ProviderController(IProviderService providerService)
    {
        _providerService = providerService;
    }

    private Guid GetProviderIdFromToken()
    {
        var providerIdClaim = User.FindFirst("sub")?.Value
       ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (providerIdClaim == null)
        {
            throw new UnauthorizedAccessException("Invalid token");
        }
        return Guid.Parse(providerIdClaim);
    }
    
    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile()
    {
        var providerId = GetProviderIdFromToken();
        var provider = await _providerService.GetProfileAsync(providerId);
        return Ok(provider);
    }

    [HttpPut("profile")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProviderRequest request)
    {
        var providerId = GetProviderIdFromToken();
        await _providerService.UpdateProfileAsync(providerId, request);
        return Ok("Profile updated successfully");
    }

    [HttpDelete("profile")]
    public async Task<IActionResult> DeleteProfile()
    {
        var providerId = GetProviderIdFromToken();
        await _providerService.DeleteProfileAsync(providerId);
        return Ok("Profile deleted successfully");
    }
}
