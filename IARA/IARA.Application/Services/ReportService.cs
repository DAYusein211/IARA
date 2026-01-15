using IARA.Application.DTOs.Report;
using IARA.Application.Interfaces;
using IARA.Domain.Enums;

namespace IARA.Application.Services;

public class ReportService : IReportService
{
    private readonly IUnitOfWork _unitOfWork;

    public ReportService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<ExpiringPermitReportDto>> GetExpiringPermitsReportAsync(int daysFromNow = 30)
    {
        var permits = await _unitOfWork.Permits.GetExpiringPermitsAsync(daysFromNow);
        
        return permits.Select(p =>
        {
            var daysUntilExpiry = (p.ValidTo - DateTime.UtcNow).Days;
            
            return new ExpiringPermitReportDto
            {
                PermitId = p.Id,
                ShipId = p.ShipId,
                ShipName = p.Ship.Name,
                ShipRegistrationNumber = p.Ship.RegistrationNumber,
                OwnerName = p.Ship.Owner.FullName,
                OwnerEmail = p.Ship.Owner.Email,
                ValidFrom = p.ValidFrom,
                ValidTo = p.ValidTo,
                DaysUntilExpiry = daysUntilExpiry > 0 ? daysUntilExpiry : 0,
                AllowedGear = p.AllowedGear
            };
        }).OrderBy(p => p.DaysUntilExpiry);
    }

    public async Task<IEnumerable<TopRecreationalFisherDto>> GetTopRecreationalFishersReportAsync(int year, int topCount = 10)
    {
        var recreationalFishers = await _unitOfWork.Users.FindAsync(u => u.Role == UserRole.RECREATIONAL_FISHER);
        
        var fisherStats = new List<TopRecreationalFisherDto>();

        foreach (var fisher in recreationalFishers)
        {
 
            
            var tickets = await _unitOfWork.Tickets.GetByUserIdWithUserAsync(fisher.Id);
            var activeTickets = tickets.Count(t => t.ValidTo >= DateTime.UtcNow && t.ValidFrom <= DateTime.UtcNow);
            var lastTicketExpiry = tickets.OrderByDescending(t => t.ValidTo).FirstOrDefault()?.ValidTo;
            
            fisherStats.Add(new TopRecreationalFisherDto
            {
                UserId = fisher.Id,
                FullName = fisher.FullName,
                Email = fisher.Email,
                TotalCatchKg = 0, 
                ActiveTickets = activeTickets,
                LastTicketExpiry = lastTicketExpiry
            });
        }

        var ranked = fisherStats
            .OrderByDescending(f => f.ActiveTickets)
            .Take(topCount)
            .Select((f, index) =>
            {
                f.Rank = index + 1;
                return f;
            });

        return ranked;
    }

    public async Task<ShipStatisticsDto> GetShipStatisticsAsync(int shipId, int year)
    {
        var ship = await _unitOfWork.Ships.GetByIdWithOwnerAsync(shipId);
        if (ship == null)
        {
            throw new InvalidOperationException("Ship not found");
        }

        var allTrips = await _unitOfWork.FishingTrips.GetByShipIdWithDetailsAsync(shipId);
        
        var yearTrips = allTrips.Where(t => t.StartTime.Year == year).ToList();
        var completedTrips = yearTrips.Where(t => t.EndTime.HasValue).ToList();

        var tripDurations = completedTrips
            .Where(t => t.EndTime.HasValue)
            .Select(t => (t.EndTime!.Value - t.StartTime).TotalHours)
            .ToList();

        var yearlyCatch = yearTrips.SelectMany(t => t.Catches).Sum(c => c.QuantityKg);
        var totalCatchAllTime = allTrips.SelectMany(t => t.Catches).Sum(c => c.QuantityKg);

        var totalFuelUsed = completedTrips.Where(t => t.FuelUsed.HasValue).Sum(t => t.FuelUsed!.Value);
        var avgFuelPerTrip = completedTrips.Any(t => t.FuelUsed.HasValue)
            ? completedTrips.Where(t => t.FuelUsed.HasValue).Average(t => t.FuelUsed!.Value)
            : (decimal?)null;

        decimal? carbonFootprintRatio = null;
        if (totalFuelUsed > 0 && yearlyCatch > 0)
        {
            carbonFootprintRatio = totalFuelUsed / yearlyCatch;
        }

        return new ShipStatisticsDto
        {
            ShipId = ship.Id,
            ShipName = ship.Name,
            RegistrationNumber = ship.RegistrationNumber,
            OwnerName = ship.Owner.FullName,
            FuelType = ship.FuelType,
            EnginePower = ship.EnginePower,
            TotalTrips = yearTrips.Count,
            CompletedTrips = completedTrips.Count,
            ActiveTrips = yearTrips.Count(t => !t.EndTime.HasValue),
            AverageTripDurationHours = tripDurations.Any() ? tripDurations.Average() : null,
            MinTripDurationHours = tripDurations.Any() ? tripDurations.Min() : null,
            MaxTripDurationHours = tripDurations.Any() ? tripDurations.Max() : null,
            YearlyCatchKg = yearlyCatch,
            TotalCatchAllTimeKg = totalCatchAllTime,
            TotalFuelUsed = totalFuelUsed,
            AverageFuelPerTrip = avgFuelPerTrip,
            CarbonFootprintRatio = carbonFootprintRatio
        };
    }

    public async Task<IEnumerable<ShipStatisticsDto>> GetAllShipsStatisticsAsync(int year)
    {
        var ships = await _unitOfWork.Ships.GetAllWithOwnersAsync();
        var statistics = new List<ShipStatisticsDto>();

        foreach (var ship in ships)
        {
            var stats = await GetShipStatisticsAsync(ship.Id, year);
            statistics.Add(stats);
        }

        return statistics.OrderByDescending(s => s.YearlyCatchKg);
    }

    public async Task<IEnumerable<CarbonFootprintDto>> GetCarbonFootprintReportAsync()
    {
        var ships = await _unitOfWork.Ships.GetAllWithOwnersAsync();
        var footprints = new List<CarbonFootprintDto>();

        foreach (var ship in ships)
        {
            var allTrips = await _unitOfWork.FishingTrips.GetByShipIdWithDetailsAsync(ship.Id);
            var completedTrips = allTrips.Where(t => t.EndTime.HasValue).ToList();

            var totalFuelUsed = completedTrips.Where(t => t.FuelUsed.HasValue).Sum(t => t.FuelUsed!.Value);
            var totalCatch = allTrips.SelectMany(t => t.Catches).Sum(c => c.QuantityKg);

            if (totalCatch > 0 && totalFuelUsed > 0)
            {
                var fuelPerCatchRatio = totalFuelUsed / totalCatch;
                
                string efficiencyRating;
                if (fuelPerCatchRatio < 0.5m)
                    efficiencyRating = "Excellent";
                else if (fuelPerCatchRatio < 1.0m)
                    efficiencyRating = "Good";
                else if (fuelPerCatchRatio < 2.0m)
                    efficiencyRating = "Average";
                else
                    efficiencyRating = "Poor";

                footprints.Add(new CarbonFootprintDto
                {
                    ShipId = ship.Id,
                    ShipName = ship.Name,
                    RegistrationNumber = ship.RegistrationNumber,
                    FuelType = ship.FuelType,
                    EnginePower = ship.EnginePower,
                    TotalFuelUsed = totalFuelUsed,
                    TotalCatchKg = totalCatch,
                    FuelPerCatchRatio = fuelPerCatchRatio,
                    EfficiencyRating = efficiencyRating
                });
            }
        }

        return footprints.OrderBy(f => f.FuelPerCatchRatio);
    }
}