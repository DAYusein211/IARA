namespace IARA.Domain.Entities;

public class FishingTrip
{
    public int Id { get; set; }
    public int ShipId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public decimal? FuelUsed { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public Ship Ship { get; set; } = null!;
    public ICollection<Catch> Catches { get; set; } = new List<Catch>();
}