namespace IARA.Domain.Entities;

public class Catch
{
    public int Id { get; set; }
    public int FishingTripId { get; set; }
    public string FishType { get; set; } = string.Empty;
    public decimal QuantityKg { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public FishingTrip FishingTrip { get; set; } = null!;
}