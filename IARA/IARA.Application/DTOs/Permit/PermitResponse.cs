namespace IARA.Application.DTOs.Permit;

public class PermitResponse
{
    public int Id { get; set; }
    public int ShipId { get; set; }
    public string ShipName { get; set; } = string.Empty;
    public string ShipRegistrationNumber { get; set; } = string.Empty;
    public DateTime ValidFrom { get; set; }
    public DateTime ValidTo { get; set; }
    public string AllowedGear { get; set; } = string.Empty;
    public bool IsExpired { get; set; }
    public int DaysUntilExpiry { get; set; }
    public DateTime CreatedAt { get; set; }
}