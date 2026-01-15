using IARA.Application.DTOs.Ticket;
using IARA.Application.Interfaces;
using IARA.Domain.Entities;
using IARA.Domain.Enums;

namespace IARA.Application.Services;

public class TicketService : ITicketService
{
    private readonly IUnitOfWork _unitOfWork;

    public TicketService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<TicketResponse> BuyTicketAsync(BuyTicketRequest request)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(request.UserId);
        if (user == null)
        {
            throw new InvalidOperationException("User not found");
        }

        if (user.Role != UserRole.RECREATIONAL_FISHER)
        {
            throw new InvalidOperationException("Only recreational fishers can purchase tickets");
        }

        var validFrom = DateTime.UtcNow;
        DateTime validTo;
        decimal price;

        switch (request.TicketType)
        {
            case TicketType.DAILY:
                validTo = validFrom.AddDays(1);
                price = 10m;
                break;
            case TicketType.WEEKLY:
                validTo = validFrom.AddDays(7);
                price = 50m;
                break;
            case TicketType.MONTHLY:
                validTo = validFrom.AddMonths(1);
                price = 150m;
                break;
            case TicketType.YEARLY:
                validTo = validFrom.AddYears(1);
                price = 1200m;
                break;
            default:
                throw new InvalidOperationException("Invalid ticket type");
        }

        var ticket = new Domain.Entities.Ticket
        {
            UserId = request.UserId,
            ValidFrom = validFrom,
            ValidTo = validTo,
            TicketType = request.TicketType,
            Price = price,
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Tickets.AddAsync(ticket);
        await _unitOfWork.SaveChangesAsync();

        // Reload with user data
        var createdTicket = await _unitOfWork.Tickets.GetByIdWithUserAsync(ticket.Id);
        return MapToResponse(createdTicket!);
    }

    public async Task<TicketResponse> GetTicketByIdAsync(int id)
    {
        var ticket = await _unitOfWork.Tickets.GetByIdWithUserAsync(id);
        if (ticket == null)
        {
            throw new InvalidOperationException("Ticket not found");
        }

        return MapToResponse(ticket);
    }

    public async Task<IEnumerable<TicketResponse>> GetAllTicketsAsync()
    {
        var tickets = await _unitOfWork.Tickets.GetAllWithUsersAsync();
        return tickets.Select(MapToResponse);
    }

    public async Task<IEnumerable<TicketResponse>> GetTicketsByUserIdAsync(int userId)
    {
        var tickets = await _unitOfWork.Tickets.GetByUserIdWithUserAsync(userId);
        return tickets.Select(MapToResponse);
    }

    public async Task<IEnumerable<TicketResponse>> GetActiveTicketsAsync()
    {
        var tickets = await _unitOfWork.Tickets.GetActiveTicketsAsync();
        return tickets.Select(MapToResponse);
    }

    public async Task<IEnumerable<TicketResponse>> GetExpiredTicketsAsync()
    {
        var tickets = await _unitOfWork.Tickets.GetExpiredTicketsAsync();
        return tickets.Select(MapToResponse);
    }

    public async Task<TicketResponse?> GetActiveTicketForUserAsync(int userId)
    {
        var ticket = await _unitOfWork.Tickets.GetActiveTicketForUserAsync(userId);
        return ticket != null ? MapToResponse(ticket) : null;
    }

    public async Task DeleteTicketAsync(int id)
    {
        var ticket = await _unitOfWork.Tickets.GetByIdAsync(id);
        if (ticket == null)
        {
            throw new InvalidOperationException("Ticket not found");
        }

        _unitOfWork.Tickets.Delete(ticket);
        await _unitOfWork.SaveChangesAsync();
    }

    private TicketResponse MapToResponse(Domain.Entities.Ticket ticket)
    {
        var now = DateTime.UtcNow;
        var isActive = ticket.ValidFrom <= now && ticket.ValidTo >= now;
        var daysRemaining = isActive ? (ticket.ValidTo - now).Days : 0;

        return new TicketResponse
        {
            Id = ticket.Id,
            UserId = ticket.UserId,
            UserName = ticket.User.FullName,
            UserEmail = ticket.User.Email,
            ValidFrom = ticket.ValidFrom,
            ValidTo = ticket.ValidTo,
            TicketType = ticket.TicketType,
            Price = ticket.Price,
            IsActive = isActive,
            DaysRemaining = daysRemaining > 0 ? daysRemaining : 0,
            CreatedAt = ticket.CreatedAt
        };
    }
}