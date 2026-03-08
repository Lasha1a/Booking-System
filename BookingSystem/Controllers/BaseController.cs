using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BookingSystem.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BaseController : ControllerBase
{
    // added basecontroller where i have method that gets user by its token 
    protected Guid GetProviderIdFromToken()
    {
        var providerIdClaim = User.FindFirst("sub")?.Value
            ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (providerIdClaim == null)
            throw new UnauthorizedAccessException("Invalid token");

        return Guid.Parse(providerIdClaim);
    }
}
