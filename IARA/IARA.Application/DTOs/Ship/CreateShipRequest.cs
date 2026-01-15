using System.ComponentModel.DataAnnotations;
using IARA.Domain.Enums;

namespace IARA.Application.DTOs.Ship;

public class CreateShipRequest
{
    [Required]
    [StringLength(200, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(50, MinimumLength = 3)]
    public string RegistrationNumber { get; set; } = string.Empty;

    [Required]
    [Range(1, int.MaxValue)]
    public int OwnerId { get; set; }

    [Required]
    [Range(0.1, 10000)]
    public decimal EnginePower { get; set; }

    [Required]
    public FuelType FuelType { get; set; }
}