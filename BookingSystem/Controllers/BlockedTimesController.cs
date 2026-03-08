using BookingSystem.Application.DTOs.BlockedTimes;
using BookingSystem.Application.Interfaces.BlockedHours;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BookingSystem.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class BlockedTimesController : BaseController
{
    private readonly IBlockedTimeService _blockedTimeService;

    public BlockedTimesController(IBlockedTimeService blockedTimeService)
    {
        _blockedTimeService = blockedTimeService;
    }

    [HttpPost]
    public async Task<IActionResult> AddBlockedTime([FromBody] AddBlockedTimeRequest request)
    {
        var providerId = GetProviderIdFromToken();
        await _blockedTimeService.AddBlockedTimeAsync(providerId, request);
        return Ok("Blocked time added successfully");
    }

    [HttpGet]
    public async Task<IActionResult> GetBlockedTimes()
    {
        var providerId = GetProviderIdFromToken();
        var blockedTimes = await _blockedTimeService.GetBlockedTimesAsync(providerId);
        return Ok(blockedTimes);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateBlockedTime(Guid id, [FromBody] UpdateBlockedTimeRequest request)
    {
        var providerId = GetProviderIdFromToken();
        await _blockedTimeService.UpdateBlockedTimeAsync(providerId, id, request);
        return Ok("Blocked time updated successfully");
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBlockedTime(Guid id)
    {
        var providerId = GetProviderIdFromToken();
        await _blockedTimeService.DeleteBlockedTimeAsync(providerId, id);
        return Ok("Blocked time deleted successfully");
    }
}
