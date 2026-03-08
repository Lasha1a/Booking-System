using BookingSystem.Application.DTOs.WorkingHours;
using BookingSystem.Application.Interfaces.WorkingHours;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BookingSystem.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class WorkingHoursController : ControllerBase
{
    private readonly IWorkingHoursService _workingHoursService;

    public WorkingHoursController(IWorkingHoursService workingHoursService)
    {
        _workingHoursService = workingHoursService;
    }

    private Guid GetProviderIdFromToken() // private method to get usdr from token itself
    {
        var providerIdClaim = User.FindFirst("sub")?.Value
            ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (providerIdClaim == null)
            throw new UnauthorizedAccessException("Invalid token");

        return Guid.Parse(providerIdClaim);
    }

    [HttpPost("add-working-hour")]
    public async Task<IActionResult> AddWorkingHours([FromBody] AddWorkingHoursRequest request)
    {
        var providerId = GetProviderIdFromToken();
        await _workingHoursService.AddWorkingHoursAsync(providerId, request);
        return Ok("Working hours added successfully");
    }

    [HttpGet("get-working-hours")]
    public async Task<IActionResult> GetWorkingHours()
    {
        var providerId = GetProviderIdFromToken();
        var workingHours = await _workingHoursService.GetWorkingHoursAsync(providerId);
        return Ok(workingHours);
    }

    [HttpPut("update{id}")]
    public async Task<IActionResult> UpdateWorkingHours(Guid id, [FromBody] UpdateWorkingHoursRequest request)
    {
        var providerId = GetProviderIdFromToken();
        await _workingHoursService.UpdateWorkingHoursAsync(providerId, id, request);
        return Ok("Working hours updated successfully");
    }

    [HttpDelete("Delete{id}")]
    public async Task<IActionResult> DeleteWorkingHours(Guid id)
    {
        var providerId = GetProviderIdFromToken();
        await _workingHoursService.DeleteWorkingHoursAsync(providerId, id);
        return Ok("Working hours deleted successfully");
    }
}
