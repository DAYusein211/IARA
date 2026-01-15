using IARA.Domain.Enums;

namespace IARA.Application.DTOs.Report;

public class CarbonFootprintDto
{
    public int ShipId { get; set; }
    public string ShipName { get; set; } = string.Empty;
    public string RegistrationNumber { get; set; } = string.Empty;
    public FuelType FuelType { get; set; }
    public decimal EnginePower { get; set; }
    public decimal TotalFuelUsed { get; set; }
    public decimal TotalCatchKg { get; set; }
    public decimal FuelPerCatchRatio { get; set; } 
    public string EfficiencyRating { get; set; } = string.Empty; 
}