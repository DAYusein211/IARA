using IARA.Domain.Enums;

namespace IARA.Domain.Entities;

public class Ticket
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public DateTime ValidFrom { get; set; }
    public DateTime ValidTo { get; set; }
    public TicketType TicketType { get; set; }
    public decimal Price { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public User User { get; set; } = null!;
}