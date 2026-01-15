using IARA.Domain.Entities;

namespace IARA.Application.Interfaces;

public interface ITicketRepository : IRepository<Ticket>
{
    Task<Ticket?> GetByIdWithUserAsync(int id);
    Task<IEnumerable<Ticket>> GetAllWithUsersAsync();
    Task<IEnumerable<Ticket>> GetByUserIdWithUserAsync(int userId);
    Task<IEnumerable<Ticket>> GetActiveTicketsAsync();
    Task<IEnumerable<Ticket>> GetExpiredTicketsAsync();
    Task<Ticket?> GetActiveTicketForUserAsync(int userId);
}