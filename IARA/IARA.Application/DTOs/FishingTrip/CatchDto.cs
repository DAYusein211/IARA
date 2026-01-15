using System.ComponentModel.DataAnnotations;

namespace IARA.Application.DTOs.FishingTrip;

public class CatchDto
{
    public int? Id { get; set; }
    
    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string FishType { get; set; } = string.Empty;

    [Required]
    [Range(0.1, 100000)]
    public decimal QuantityKg { get; set; }
}