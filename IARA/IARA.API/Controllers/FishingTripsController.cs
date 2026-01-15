using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using IARA.Application.DTOs.FishingTrip;
using IARA.Application.Interfaces;

namespace IARA.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FishingTripsController : ControllerBase
{
    private readonly IFishingTripService _fishingTripService;
    private readonly ILogger<FishingTripsController> _logger;

    public FishingTripsController(IFishingTripService fishingTripService, ILogger<FishingTripsController> logger)
    {
        _fishingTripService = fishingTripService;
        _logger = logger;
    }


    [HttpPost]
    [Authorize(Roles = "ADMIN,PROFESSIONAL_FISHER")]
    public async Task<IActionResult> CreateFishingTrip([FromBody] CreateFishingTripRequest request)
    {
        try
        {
            var response = await _fishingTripService.CreateFishingTripAsync(request);
            return CreatedAtAction(nameof(GetFishingTripById), new { id = response.Id }, response);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating fishing trip");
            return StatusCode(500, new { message = "An error occurred while creating the fishing trip" });
        }
    }


    [HttpGet("{id}")]
    public async Task<IActionResult> GetFishingTripById(int id)
    {
        try
        {
            var response = await _fishingTripService.GetFishingTripByIdAsync(id);
            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving fishing trip {TripId}", id);
            return StatusCode(500, new { message = "An error occurred while retrieving the fishing trip" });
        }
    }

  
    [HttpGet]
    [Authorize(Roles = "ADMIN,INSPECTOR")]
    public async Task<IActionResult> GetAllFishingTrips()
    {
        try
        {
            var response = await _fishingTripService.GetAllFishingTripsAsync();
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving fishing trips");
            return StatusCode(500, new { message = "An error occurred while retrieving fishing trips" });
        }
    }

    
    [HttpGet("ship/{shipId}")]
    public async Task<IActionResult> GetFishingTripsByShipId(int shipId)
    {
        try
        {
            var response = await _fishingTripService.GetFishingTripsByShipIdAsync(shipId);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving fishing trips for ship {ShipId}", shipId);
            return StatusCode(500, new { message = "An error occurred while retrieving fishing trips" });
        }
    }


    [HttpGet("active")]
    [Authorize(Roles = "ADMIN,INSPECTOR,PROFESSIONAL_FISHER")]
    public async Task<IActionResult> GetActiveFishingTrips()
    {
        try
        {
            var response = await _fishingTripService.GetActiveFishingTripsAsync();
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving active fishing trips");
            return StatusCode(500, new { message = "An error occurred while retrieving active fishing trips" });
        }
    }

    [HttpGet("completed")]
    [Authorize(Roles = "ADMIN,INSPECTOR,PROFESSIONAL_FISHER")]
    public async Task<IActionResult> GetCompletedFishingTrips()
    {
        try
        {
            var response = await _fishingTripService.GetCompletedFishingTripsAsync();
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving completed fishing trips");
            return StatusCode(500, new { message = "An error occurred while retrieving completed fishing trips" });
        }
    }


    [HttpPut("{id}")]
    [Authorize(Roles = "ADMIN,PROFESSIONAL_FISHER")]
    public async Task<IActionResult> UpdateFishingTrip(int id, [FromBody] UpdateFishingTripRequest request)
    {
        try
        {
            var response = await _fishingTripService.UpdateFishingTripAsync(id, request);
            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating fishing trip {TripId}", id);
            return StatusCode(500, new { message = "An error occurred while updating the fishing trip" });
        }
    }


    [HttpPost("{id}/complete")]
    [Authorize(Roles = "ADMIN,PROFESSIONAL_FISHER")]
    public async Task<IActionResult> CompleteFishingTrip(int id, [FromQuery] decimal? fuelUsed = null)
    {
        try
        {
            var response = await _fishingTripService.CompleteFishingTripAsync(id, fuelUsed);
            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error completing fishing trip {TripId}", id);
            return StatusCode(500, new { message = "An error occurred while completing the fishing trip" });
        }
    }

    /// <summary>
    /// Delete fishing trip (ADMIN only)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "ADMIN,PROFESSIONAL_FISHER")]
    public async Task<IActionResult> DeleteFishingTrip(int id)
    {
        try
        {
            await _fishingTripService.DeleteFishingTripAsync(id);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting fishing trip {TripId}", id);
            return StatusCode(500, new { message = "An error occurred while deleting the fishing trip" });
        }
    }
}