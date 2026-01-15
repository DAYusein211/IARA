using System.ComponentModel.DataAnnotations;
using IARA.Domain.Enums;

namespace IARA.Application.DTOs.Inspection;

public class UpdateInspectionRequest
{
    [Required]
    public InspectionResult Result { get; set; }

    [StringLength(1000)]
    public string Notes { get; set; } = string.Empty;

    public FineDto? Fine { get; set; }
}