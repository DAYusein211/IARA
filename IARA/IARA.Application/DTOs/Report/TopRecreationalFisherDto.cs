namespace IARA.Application.DTOs.Report;

public class TopRecreationalFisherDto
{
    public int Rank { get; set; }
    public int UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public decimal TotalCatchKg { get; set; }
    public int ActiveTickets { get; set; }
    public DateTime? LastTicketExpiry { get; set; }
}