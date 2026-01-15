using IARA.Domain.Enums;

namespace IARA.Application.DTOs.Report;

public class ShipStatisticsDto
{
    public int ShipId { get; set; }
    public string ShipName { get; set; } = string.Empty;
    public string RegistrationNumber { get; set; } = string.Empty;
    public string OwnerName { get; set; } = string.Empty;
    public FuelType FuelType { get; set; }
    public decimal EnginePower { get; set; }
    
    public int TotalTrips { get; set; }
    public int CompletedTrips { get; set; }
    public int ActiveTrips { get; set; }
    public double? AverageTripDurationHours { get; set; }
    public double? MinTripDurationHours { get; set; }
    public double? MaxTripDurationHours { get; set; }
    
    public decimal YearlyCatchKg { get; set; }
    public decimal TotalCatchAllTimeKg { get; set; }
    
    public decimal? TotalFuelUsed { get; set; }
    public decimal? AverageFuelPerTrip { get; set; }
    
    public decimal? CarbonFootprintRatio { get; set; }
}