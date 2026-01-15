using Microsoft.EntityFrameworkCore;
using IARA.Application.Interfaces;
using IARA.Domain.Entities;
using IARA.Infrastructure.Data;

namespace IARA.Infrastructure.Repositories;

public class FishingTripRepository : Repository<FishingTrip>, IFishingTripRepository
{
    public FishingTripRepository(IaraDbContext context) : base(context)
    {
    }

    public async Task<FishingTrip?> GetByIdWithDetailsAsync(int id)
    {
        return await _context.FishingTrips
            .Include(t => t.Ship)
            .Include(t => t.Catches)
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<IEnumerable<FishingTrip>> GetAllWithDetailsAsync()
    {
        return await _context.FishingTrips
            .Include(t => t.Ship)
            .Include(t => t.Catches)
            .OrderByDescending(t => t.StartTime)
            .ToListAsync();
    }

    public async Task<IEnumerable<FishingTrip>> GetByShipIdWithDetailsAsync(int shipId)
    {
        return await _context.FishingTrips
            .Include(t => t.Ship)
            .Include(t => t.Catches)
            .Where(t => t.ShipId == shipId)
            .OrderByDescending(t => t.StartTime)
            .ToListAsync();
    }

    public async Task<IEnumerable<FishingTrip>> GetActiveTripsAsync()
    {
        var now = DateTime.UtcNow;
        return await _context.FishingTrips
            .Include(t => t.Ship)
            .Include(t => t.Catches)
            .Where(t => t.EndTime == null || t.EndTime > now)
            .OrderByDescending(t => t.StartTime)
            .ToListAsync();
    }

    public async Task<IEnumerable<FishingTrip>> GetCompletedTripsAsync()
    {
        var now = DateTime.UtcNow;
        return await _context.FishingTrips
            .Include(t => t.Ship)
            .Include(t => t.Catches)
            .Where(t => t.EndTime != null && (t.EndTime <= now || t.EndTime == t.StartTime))
            .OrderByDescending(t => t.EndTime)
            .ToListAsync();
    }

    public async Task<IEnumerable<FishingTrip>> GetTripsByDateRangeAsync(DateTime from, DateTime to)
    {
        return await _context.FishingTrips
            .Include(t => t.Ship)
            .Include(t => t.Catches)
            .Where(t => t.StartTime >= from && t.StartTime <= to)
            .OrderByDescending(t => t.StartTime)
            .ToListAsync();
    }
}