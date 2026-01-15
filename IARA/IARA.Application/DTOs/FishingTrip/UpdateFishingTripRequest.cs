using System.ComponentModel.DataAnnotations;

namespace IARA.Application.DTOs.FishingTrip;

public class UpdateFishingTripRequest
{
    public DateTime? EndTime { get; set; }

    [Range(0, 100000)]
    public decimal? FuelUsed { get; set; }

    public List<CatchDto> Catches { get; set; } = new();
}