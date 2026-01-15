using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using IARA.Application.DTOs.Ticket;
using IARA.Application.Interfaces;

namespace IARA.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TicketsController : ControllerBase
{
    private readonly ITicketService _ticketService;
    private readonly ILogger<TicketsController> _logger;

    public TicketsController(ITicketService ticketService, ILogger<TicketsController> logger)
    {
        _ticketService = ticketService;
        _logger = logger;
    }

    [HttpPost("buy")]
    [Authorize(Roles = "RECREATIONAL_FISHER")]
    public async Task<IActionResult> BuyTicket([FromBody] BuyTicketRequest request)
    {
        try
        {
            var response = await _ticketService.BuyTicketAsync(request);
            return CreatedAtAction(nameof(GetTicketById), new { id = response.Id }, response);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error buying ticket");
            return StatusCode(500, new { message = "An error occurred while purchasing the ticket" });
        }
    }


    [HttpGet("{id}")]
    public async Task<IActionResult> GetTicketById(int id)
    {
        try
        {
            var response = await _ticketService.GetTicketByIdAsync(id);
            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving ticket {TicketId}", id);
            return StatusCode(500, new { message = "An error occurred while retrieving the ticket" });
        }
    }


    [HttpGet]
    [Authorize(Roles = "ADMIN,INSPECTOR")]
    public async Task<IActionResult> GetAllTickets()
    {
        try
        {
            var response = await _ticketService.GetAllTicketsAsync();
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving tickets");
            return StatusCode(500, new { message = "An error occurred while retrieving tickets" });
        }
    }


    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetTicketsByUserId(int userId)
    {
        try
        {
            var response = await _ticketService.GetTicketsByUserIdAsync(userId);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving tickets for user {UserId}", userId);
            return StatusCode(500, new { message = "An error occurred while retrieving tickets" });
        }
    }

    [HttpGet("active")]
    [Authorize(Roles = "ADMIN,INSPECTOR")]
    public async Task<IActionResult> GetActiveTickets()
    {
        try
        {
            var response = await _ticketService.GetActiveTicketsAsync();
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving active tickets");
            return StatusCode(500, new { message = "An error occurred while retrieving active tickets" });
        }
    }


    [HttpGet("expired")]
    [Authorize(Roles = "ADMIN,INSPECTOR")]
    public async Task<IActionResult> GetExpiredTickets()
    {
        try
        {
            var response = await _ticketService.GetExpiredTicketsAsync();
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving expired tickets");
            return StatusCode(500, new { message = "An error occurred while retrieving expired tickets" });
        }
    }


    [HttpGet("user/{userId}/active")]
    public async Task<IActionResult> GetActiveTicketForUser(int userId)
    {
        try
        {
            var response = await _ticketService.GetActiveTicketForUserAsync(userId);
            if (response == null)
            {
                return NotFound(new { message = "No active ticket found for this user" });
            }
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving active ticket for user {UserId}", userId);
            return StatusCode(500, new { message = "An error occurred while retrieving the active ticket" });
        }
    }


    [HttpDelete("{id}")]
    [Authorize(Roles = "ADMIN,RECREATIONAL_FISHER")]
    public async Task<IActionResult> DeleteTicket(int id)
    {
        try
        {
            await _ticketService.DeleteTicketAsync(id);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting ticket {TicketId}", id);
            return StatusCode(500, new { message = "An error occurred while deleting the ticket" });
        }
    }
}