using Microsoft.EntityFrameworkCore;
using IARA.Application.Interfaces;
using IARA.Domain.Entities;
using IARA.Infrastructure.Data;

namespace IARA.Infrastructure.Repositories;

public class TicketRepository : Repository<Ticket>, ITicketRepository
{
    public TicketRepository(IaraDbContext context) : base(context)
    {
    }

    public async Task<Ticket?> GetByIdWithUserAsync(int id)
    {
        return await _context.Tickets
            .Include(t => t.User)
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<IEnumerable<Ticket>> GetAllWithUsersAsync()
    {
        return await _context.Tickets
            .Include(t => t.User)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Ticket>> GetByUserIdWithUserAsync(int userId)
    {
        return await _context.Tickets
            .Include(t => t.User)
            .Where(t => t.UserId == userId)
            .OrderByDescending(t => t.ValidTo)
            .ToListAsync();
    }

    public async Task<IEnumerable<Ticket>> GetActiveTicketsAsync()
    {
        var now = DateTime.UtcNow;
        
        return await _context.Tickets
            .Include(t => t.User)
            .Where(t => t.ValidFrom <= now && t.ValidTo >= now)
            .ToListAsync();
    }

    public async Task<IEnumerable<Ticket>> GetExpiredTicketsAsync()
    {
        var now = DateTime.UtcNow;
        
        return await _context.Tickets
            .Include(t => t.User)
            .Where(t => t.ValidTo < now)
            .ToListAsync();
    }

    public async Task<Ticket?> GetActiveTicketForUserAsync(int userId)
    {
        var now = DateTime.UtcNow;
        
        return await _context.Tickets
            .Include(t => t.User)
            .Where(t => t.UserId == userId && t.ValidFrom <= now && t.ValidTo >= now)
            .OrderByDescending(t => t.ValidTo)
            .FirstOrDefaultAsync();
    }
}