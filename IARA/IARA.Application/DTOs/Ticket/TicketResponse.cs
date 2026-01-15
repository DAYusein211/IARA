using IARA.Domain.Enums;

namespace IARA.Application.DTOs.Ticket;

public class TicketResponse
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string UserEmail { get; set; } = string.Empty;
    public DateTime ValidFrom { get; set; }
    public DateTime ValidTo { get; set; }
    public TicketType TicketType { get; set; }
    public decimal Price { get; set; }
    public bool IsActive { get; set; }
    public int DaysRemaining { get; set; }
    public DateTime CreatedAt { get; set; }
}