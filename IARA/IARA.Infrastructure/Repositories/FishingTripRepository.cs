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
        return await _context.FishingTrips
            .Include(t => t.Ship)
            .Include(t => t.Catches)
            .Where(t => t.EndTime == null)
            .OrderByDescending(t => t.StartTime)
            .ToListAsync();
    }

    public async Task<IEnumerable<FishingTrip>> GetCompletedTripsAsync()
    {
        return await _context.FishingTrips
            .Include(t => t.Ship)
            .Include(t => t.Catches)
            .Where(t => t.EndTime != null)
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