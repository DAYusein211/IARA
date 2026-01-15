using IARA.Domain.Enums;

namespace IARA.Domain.Entities;

public class User
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public ICollection<Ship> Ships { get; set; } = new List<Ship>();
    public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
    public ICollection<Inspection> Inspections { get; set; } = new List<Inspection>();
}