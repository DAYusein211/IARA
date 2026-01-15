using System.ComponentModel.DataAnnotations;

namespace IARA.Application.DTOs.FishingTrip;

public class CreateFishingTripRequest
{
    [Required]
    [Range(1, int.MaxValue)]
    public int ShipId { get; set; }

    [Required]
    public DateTime StartTime { get; set; }

    public DateTime? EndTime { get; set; }

    [Range(0, 100000)]
    public decimal? FuelUsed { get; set; }

    public List<CatchDto> Catches { get; set; } = new();
}