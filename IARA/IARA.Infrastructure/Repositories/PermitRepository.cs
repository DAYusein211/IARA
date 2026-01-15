using Microsoft.EntityFrameworkCore;
using IARA.Application.Interfaces;
using IARA.Domain.Entities;
using IARA.Infrastructure.Data;

namespace IARA.Infrastructure.Repositories;

public class PermitRepository : Repository<Permit>, IPermitRepository
{
    public PermitRepository(IaraDbContext context) : base(context)
    {
    }

    public async Task<Permit?> GetByIdWithShipAsync(int id)
    {
        return await _context.Permits
            .Include(p => p.Ship)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<IEnumerable<Permit>> GetAllWithShipsAsync()
    {
        return await _context.Permits
            .Include(p => p.Ship)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Permit>> GetByShipIdWithShipAsync(int shipId)
    {
        return await _context.Permits
            .Include(p => p.Ship)
            .Where(p => p.ShipId == shipId)
            .OrderByDescending(p => p.ValidTo)
            .ToListAsync();
    }

    public async Task<IEnumerable<Permit>> GetExpiringPermitsAsync(int daysFromNow)
    {
        var fromDate = DateTime.UtcNow;
        var toDate = DateTime.UtcNow.AddDays(daysFromNow);

        return await _context.Permits
            .Include(p => p.Ship)
                .ThenInclude(s => s.Owner)
            .Where(p => p.ValidTo >= fromDate && p.ValidTo <= toDate)
            .OrderBy(p => p.ValidTo)
            .ToListAsync();
    }

    public async Task<IEnumerable<Permit>> GetActivePermitsAsync()
    {
        var now = DateTime.UtcNow;
        
        return await _context.Permits
            .Include(p => p.Ship)
            .Where(p => p.ValidFrom <= now && p.ValidTo >= now)
            .ToListAsync();
    }

    public async Task<IEnumerable<Permit>> GetExpiredPermitsAsync()
    {
        var now = DateTime.UtcNow;
        
        return await _context.Permits
            .Include(p => p.Ship)
            .Where(p => p.ValidTo < now)
            .ToListAsync();
    }
}