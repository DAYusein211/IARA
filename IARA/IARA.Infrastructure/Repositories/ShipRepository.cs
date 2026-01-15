using Microsoft.EntityFrameworkCore;
using IARA.Application.Interfaces;
using IARA.Domain.Entities;
using IARA.Infrastructure.Data;

namespace IARA.Infrastructure.Repositories;

public class ShipRepository : Repository<Ship>, IShipRepository
{
    public ShipRepository(IaraDbContext context) : base(context)
    {
    }

    public async Task<Ship?> GetByIdWithOwnerAsync(int id)
    {
        return await _context.Ships
            .Include(s => s.Owner)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<IEnumerable<Ship>> GetAllWithOwnersAsync()
    {
        return await _context.Ships
            .Include(s => s.Owner)
            .ToListAsync();
    }

    public async Task<IEnumerable<Ship>> GetByOwnerIdWithOwnerAsync(int ownerId)
    {
        return await _context.Ships
            .Include(s => s.Owner)
            .Where(s => s.OwnerId == ownerId)
            .ToListAsync();
    }

    public async Task<bool> IsRegistrationNumberUniqueAsync(string registrationNumber, int? excludeShipId = null)
    {
        var query = _context.Ships.Where(s => s.RegistrationNumber == registrationNumber);
        
        if (excludeShipId.HasValue)
        {
            query = query.Where(s => s.Id != excludeShipId.Value);
        }
        
        return !await query.AnyAsync();
    }
}