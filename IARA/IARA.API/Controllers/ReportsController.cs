using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using IARA.Application.Interfaces;

namespace IARA.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "ADMIN,INSPECTOR")]
public class ReportsController : ControllerBase
{
    private readonly IReportService _reportService;
    private readonly ILogger<ReportsController> _logger;

    public ReportsController(IReportService reportService, ILogger<ReportsController> logger)
    {
        _reportService = reportService;
        _logger = logger;
    }

   
    [HttpGet("expiring-permits")]
    public async Task<IActionResult> GetExpiringPermitsReport([FromQuery] int days = 30)
    {
        try
        {
            var response = await _reportService.GetExpiringPermitsReportAsync(days);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating expiring permits report");
            return StatusCode(500, new { message = "An error occurred while generating the report" });
        }
    }

 
    [HttpGet("top-recreational-fishers")]
    public async Task<IActionResult> GetTopRecreationalFishersReport([FromQuery] int year, [FromQuery] int top = 10)
    {
        try
        {
            var response = await _reportService.GetTopRecreationalFishersReportAsync(year, top);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating top recreational fishers report");
            return StatusCode(500, new { message = "An error occurred while generating the report" });
        }
    }

    [HttpGet("ship-statistics/{shipId}")]
    public async Task<IActionResult> GetShipStatistics(int shipId, [FromQuery] int year)
    {
        try
        {
            var response = await _reportService.GetShipStatisticsAsync(shipId, year);
            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating ship statistics for ship {ShipId}", shipId);
            return StatusCode(500, new { message = "An error occurred while generating the report" });
        }
    }


    [HttpGet("all-ships-statistics")]
    public async Task<IActionResult> GetAllShipsStatistics([FromQuery] int year)
    {
        try
        {
            var response = await _reportService.GetAllShipsStatisticsAsync(year);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating all ships statistics");
            return StatusCode(500, new { message = "An error occurred while generating the report" });
        }
    }


    [HttpGet("carbon-footprint")]
    public async Task<IActionResult> GetCarbonFootprintReport()
    {
        try
        {
            var response = await _reportService.GetCarbonFootprintReportAsync();
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating carbon footprint report");
            return StatusCode(500, new { message = "An error occurred while generating the report" });
        }
    }
}