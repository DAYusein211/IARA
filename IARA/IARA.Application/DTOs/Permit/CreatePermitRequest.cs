using System.ComponentModel.DataAnnotations;

namespace IARA.Application.DTOs.Permit;

public class CreatePermitRequest
{
    [Required]
    [Range(1, int.MaxValue)]
    public int ShipId { get; set; }

    [Required]
    public DateTime ValidFrom { get; set; }

    [Required]
    public DateTime ValidTo { get; set; }

    [Required]
    [StringLength(500, MinimumLength = 3)]
    public string AllowedGear { get; set; } = string.Empty;
}