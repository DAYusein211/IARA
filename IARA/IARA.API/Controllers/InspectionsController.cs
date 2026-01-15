using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using IARA.Application.DTOs.Inspection;
using IARA.Application.Interfaces;
using IARA.Domain.Enums;

namespace IARA.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class InspectionsController : ControllerBase
{
    private readonly IInspectionService _inspectionService;
    private readonly ILogger<InspectionsController> _logger;

    public InspectionsController(IInspectionService inspectionService, ILogger<InspectionsController> logger)
    {
        _inspectionService = inspectionService;
        _logger = logger;
    }
    
    [HttpPost]
    [Authorize(Roles = "ADMIN,INSPECTOR")]
    public async Task<IActionResult> CreateInspection([FromBody] CreateInspectionRequest request)
    {
        try
        {
            var response = await _inspectionService.CreateInspectionAsync(request);
            return CreatedAtAction(nameof(GetInspectionById), new { id = response.Id }, response);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating inspection");
            return StatusCode(500, new { message = "An error occurred while creating the inspection" });
        }
    }
    
    [HttpGet("{id}")]
    [Authorize(Roles = "ADMIN,INSPECTOR")]
    public async Task<IActionResult> GetInspectionById(int id)
    {
        try
        {
            var response = await _inspectionService.GetInspectionByIdAsync(id);
            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving inspection {InspectionId}", id);
            return StatusCode(500, new { message = "An error occurred while retrieving the inspection" });
        }
    }


    [HttpGet]
    [Authorize(Roles = "ADMIN,INSPECTOR")]
    public async Task<IActionResult> GetAllInspections()
    {
        try
        {
            var response = await _inspectionService.GetAllInspectionsAsync();
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving inspections");
            return StatusCode(500, new { message = "An error occurred while retrieving inspections" });
        }
    }

    [HttpGet("inspector/{inspectorId}")]
    [Authorize(Roles = "ADMIN,INSPECTOR")]
    public async Task<IActionResult> GetInspectionsByInspectorId(int inspectorId)
    {
        try
        {
            var response = await _inspectionService.GetInspectionsByInspectorIdAsync(inspectorId);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving inspections for inspector {InspectorId}", inspectorId);
            return StatusCode(500, new { message = "An error occurred while retrieving inspections" });
        }
    }

    [HttpGet("target")]
    [Authorize(Roles = "ADMIN,INSPECTOR")]
    public async Task<IActionResult> GetInspectionsByTarget([FromQuery] int targetType, [FromQuery] int targetId)
    {
        try
        {
            var response = await _inspectionService.GetInspectionsByTargetAsync((TargetType)targetType, targetId);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving inspections for target {TargetType}/{TargetId}", targetType, targetId);
            return StatusCode(500, new { message = "An error occurred while retrieving inspections" });
        }
    }

  
    [HttpGet("result/{result}")]
    [Authorize(Roles = "ADMIN,INSPECTOR")]
    public async Task<IActionResult> GetInspectionsByResult(int result)
    {
        try
        {
            var response = await _inspectionService.GetInspectionsByResultAsync((InspectionResult)result);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving inspections by result {Result}", result);
            return StatusCode(500, new { message = "An error occurred while retrieving inspections" });
        }
    }

    [HttpGet("with-fines")]
    [Authorize(Roles = "ADMIN,INSPECTOR")]
    public async Task<IActionResult> GetInspectionsWithFines()
    {
        try
        {
            var response = await _inspectionService.GetInspectionsWithFinesAsync();
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving inspections with fines");
            return StatusCode(500, new { message = "An error occurred while retrieving inspections with fines" });
        }
    }


    [HttpGet("unpaid-fines")]
    [Authorize(Roles = "ADMIN,INSPECTOR")]
    public async Task<IActionResult> GetInspectionsWithUnpaidFines()
    {
        try
        {
            var response = await _inspectionService.GetInspectionsWithUnpaidFinesAsync();
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving inspections with unpaid fines");
            return StatusCode(500, new { message = "An error occurred while retrieving unpaid fines" });
        }
    }


    [HttpPut("{id}")]
    [Authorize(Roles = "ADMIN,INSPECTOR")]
    public async Task<IActionResult> UpdateInspection(int id, [FromBody] UpdateInspectionRequest request)
    {
        try
        {
            var response = await _inspectionService.UpdateInspectionAsync(id, request);
            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating inspection {InspectionId}", id);
            return StatusCode(500, new { message = "An error occurred while updating the inspection" });
        }
    }

    [HttpPost("{id}/pay-fine")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> MarkFineAsPaid(int id)
    {
        try
        {
            var response = await _inspectionService.MarkFineAsPaidAsync(id);
            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking fine as paid for inspection {InspectionId}", id);
            return StatusCode(500, new { message = "An error occurred while marking fine as paid" });
        }
    }


    [HttpDelete("{id}")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> DeleteInspection(int id)
    {
        try
        {
            await _inspectionService.DeleteInspectionAsync(id);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting inspection {InspectionId}", id);
            return StatusCode(500, new { message = "An error occurred while deleting the inspection" });
        }
    }
}