using IARA.Application.DTOs.Ticket;

namespace IARA.Application.Interfaces;

public interface ITicketService
{
    Task<TicketResponse> BuyTicketAsync(BuyTicketRequest request);
    Task<TicketResponse> GetTicketByIdAsync(int id);
    Task<IEnumerable<TicketResponse>> GetAllTicketsAsync();
    Task<IEnumerable<TicketResponse>> GetTicketsByUserIdAsync(int userId);
    Task<IEnumerable<TicketResponse>> GetActiveTicketsAsync();
    Task<IEnumerable<TicketResponse>> GetExpiredTicketsAsync();
    Task<TicketResponse?> GetActiveTicketForUserAsync(int userId);
    Task DeleteTicketAsync(int id);
}