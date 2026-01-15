using IARA.Domain.Enums;

namespace IARA.Application.DTOs.Ship;

public class ShipResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string RegistrationNumber { get; set; } = string.Empty;
    public int OwnerId { get; set; }
    public string OwnerName { get; set; } = string.Empty;
    public decimal EnginePower { get; set; }
    public FuelType FuelType { get; set; }
    public DateTime CreatedAt { get; set; }
}