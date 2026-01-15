using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using IARA.Application.DTOs.Permit;
using IARA.Application.Interfaces;

namespace IARA.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PermitsController : ControllerBase
{
    private readonly IPermitService _permitService;
    private readonly ILogger<PermitsController> _logger;

    public PermitsController(IPermitService permitService, ILogger<PermitsController> logger)
    {
        _permitService = permitService;
        _logger = logger;
    }

 
    [HttpPost]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> CreatePermit([FromBody] CreatePermitRequest request)
    {
        try
        {
            var response = await _permitService.CreatePermitAsync(request);
            return CreatedAtAction(nameof(GetPermitById), new { id = response.Id }, response);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating permit");
            return StatusCode(500, new { message = "An error occurred while creating the permit" });
        }
    }


    [HttpGet("{id}")]
    public async Task<IActionResult> GetPermitById(int id)
    {
        try
        {
            var response = await _permitService.GetPermitByIdAsync(id);
            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving permit {PermitId}", id);
            return StatusCode(500, new { message = "An error occurred while retrieving the permit" });
        }
    }

    [HttpGet]
    [Authorize(Roles = "ADMIN,INSPECTOR")]
    public async Task<IActionResult> GetAllPermits()
    {
        try
        {
            var response = await _permitService.GetAllPermitsAsync();
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving permits");
            return StatusCode(500, new { message = "An error occurred while retrieving permits" });
        }
    }

    [HttpGet("ship/{shipId}")]
    public async Task<IActionResult> GetPermitsByShipId(int shipId)
    {
        try
        {
            var response = await _permitService.GetPermitsByShipIdAsync(shipId);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving permits for ship {ShipId}", shipId);
            return StatusCode(500, new { message = "An error occurred while retrieving permits" });
        }
    }


    [HttpGet("expiring")]
    [Authorize(Roles = "ADMIN,INSPECTOR")]
    public async Task<IActionResult> GetExpiringPermits([FromQuery] int days = 30)
    {
        try
        {
            var response = await _permitService.GetExpiringPermitsAsync(days);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving expiring permits");
            return StatusCode(500, new { message = "An error occurred while retrieving expiring permits" });
        }
    }


    [HttpGet("active")]
    [Authorize(Roles = "ADMIN,INSPECTOR")]
    public async Task<IActionResult> GetActivePermits()
    {
        try
        {
            var response = await _permitService.GetActivePermitsAsync();
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving active permits");
            return StatusCode(500, new { message = "An error occurred while retrieving active permits" });
        }
    }


    [HttpGet("expired")]
    [Authorize(Roles = "ADMIN,INSPECTOR")]
    public async Task<IActionResult> GetExpiredPermits()
    {
        try
        {
            var response = await _permitService.GetExpiredPermitsAsync();
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving expired permits");
            return StatusCode(500, new { message = "An error occurred while retrieving expired permits" });
        }
    }


    [HttpPut("{id}")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> UpdatePermit(int id, [FromBody] UpdatePermitRequest request)
    {
        try
        {
            var response = await _permitService.UpdatePermitAsync(id, request);
            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating permit {PermitId}", id);
            return StatusCode(500, new { message = "An error occurred while updating the permit" });
        }
    }


    [HttpDelete("{id}")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> DeletePermit(int id)
    {
        try
        {
            await _permitService.DeletePermitAsync(id);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting permit {PermitId}", id);
            return StatusCode(500, new { message = "An error occurred while deleting the permit" });
        }
    }
}