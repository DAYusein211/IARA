namespace IARA.Domain.Entities;

public class Permit
{
    public int Id { get; set; }
    public int ShipId { get; set; }
    public DateTime ValidFrom { get; set; }
    public DateTime ValidTo { get; set; }
    public string AllowedGear { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public Ship Ship { get; set; } = null!;
}