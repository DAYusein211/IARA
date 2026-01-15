using IARA.Domain.Enums;

namespace IARA.Domain.Entities;

public class Ship
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string RegistrationNumber { get; set; } = string.Empty;
    public int OwnerId { get; set; }
    public decimal EnginePower { get; set; }
    public FuelType FuelType { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public User Owner { get; set; } = null!;
    public ICollection<Permit> Permits { get; set; } = new List<Permit>();
    public ICollection<FishingTrip> FishingTrips { get; set; } = new List<FishingTrip>();
}