namespace IARA.Application.DTOs.FishingTrip;

public class FishingTripResponse
{
    public int Id { get; set; }
    public int ShipId { get; set; }
    public string ShipName { get; set; } = string.Empty;
    public string ShipRegistrationNumber { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public decimal? FuelUsed { get; set; }
    public bool IsCompleted { get; set; }
    public double? DurationHours { get; set; }
    public decimal TotalCatchKg { get; set; }
    public List<CatchResponse> Catches { get; set; } = new();
    public DateTime CreatedAt { get; set; }
}

public class CatchResponse
{
    public int Id { get; set; }
    public int FishingTripId { get; set; }
    public string FishType { get; set; } = string.Empty;
    public decimal QuantityKg { get; set; }
    public DateTime CreatedAt { get; set; }
}