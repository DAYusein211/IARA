using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using IARA.Application.DTOs.Ship;
using IARA.Application.Interfaces;

namespace IARA.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ShipsController : ControllerBase
{
    private readonly IShipService _shipService;
    private readonly ILogger<ShipsController> _logger;

    public ShipsController(IShipService shipService, ILogger<ShipsController> logger)
    {
        _shipService = shipService;
        _logger = logger;
    }

    [HttpPost]
    [Authorize(Roles = "ADMIN,PROFESSIONAL_FISHER")]
    public async Task<IActionResult> CreateShip([FromBody] CreateShipRequest request)
    {
        try
        {
            var response = await _shipService.CreateShipAsync(request);
            return CreatedAtAction(nameof(GetShipById), new { id = response.Id }, response);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating ship");
            return StatusCode(500, new { message = "An error occurred while creating the ship" });
        }
    }

  
    [HttpGet("{id}")]
    public async Task<IActionResult> GetShipById(int id)
    {
        try
        {
            var response = await _shipService.GetShipByIdAsync(id);
            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving ship {ShipId}", id);
            return StatusCode(500, new { message = "An error occurred while retrieving the ship" });
        }
    }

    [HttpGet]
    [Authorize(Roles = "ADMIN,INSPECTOR")]
    public async Task<IActionResult> GetAllShips()
    {
        try
        {
            var response = await _shipService.GetAllShipsAsync();
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving ships");
            return StatusCode(500, new { message = "An error occurred while retrieving ships" });
        }
    }


    [HttpGet("owner/{ownerId}")]
    public async Task<IActionResult> GetShipsByOwnerId(int ownerId)
    {
        try
        {
            var response = await _shipService.GetShipsByOwnerIdAsync(ownerId);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving ships for owner {OwnerId}", ownerId);
            return StatusCode(500, new { message = "An error occurred while retrieving ships" });
        }
    }


    [HttpPut("{id}")]
    [Authorize(Roles = "ADMIN,PROFESSIONAL_FISHER")]
    public async Task<IActionResult> UpdateShip(int id, [FromBody] UpdateShipRequest request)
    {
        try
        {
            var response = await _shipService.UpdateShipAsync(id, request);
            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating ship {ShipId}", id);
            return StatusCode(500, new { message = "An error occurred while updating the ship" });
        }
    }


    [HttpDelete("{id}")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> DeleteShip(int id)
    {
        try
        {
            await _shipService.DeleteShipAsync(id);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting ship {ShipId}", id);
            return StatusCode(500, new { message = "An error occurred while deleting the ship" });
        }
    }
}