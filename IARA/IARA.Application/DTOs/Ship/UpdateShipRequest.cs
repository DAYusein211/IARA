using System.ComponentModel.DataAnnotations;
using IARA.Domain.Enums;

namespace IARA.Application.DTOs.Ship;

public class UpdateShipRequest
{
    [Required]
    [StringLength(200, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [Range(0.1, 10000)]
    public decimal EnginePower { get; set; }

    [Required]
    public FuelType FuelType { get; set; }
}