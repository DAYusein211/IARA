using System.ComponentModel.DataAnnotations;
using IARA.Domain.Enums;

namespace IARA.Application.DTOs.Inspection;

public class CreateInspectionRequest
{
    [Required]
    [Range(1, int.MaxValue)]
    public int InspectorId { get; set; }

    [Required]
    public TargetType TargetType { get; set; }

    [Required]
    [Range(1, int.MaxValue)]
    public int TargetId { get; set; }

    [Required]
    public DateTime InspectionDate { get; set; }

    [Required]
    public InspectionResult Result { get; set; }

    [StringLength(1000)]
    public string Notes { get; set; } = string.Empty;

    public FineDto? Fine { get; set; }
}

public class FineDto
{
    [Required]
    [Range(0.01, 1000000)]
    public decimal Amount { get; set; }

    [Required]
    [StringLength(500, MinimumLength = 3)]
    public string Reason { get; set; } = string.Empty;
}