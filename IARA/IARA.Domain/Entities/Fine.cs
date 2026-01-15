namespace IARA.Domain.Entities;

public class Fine
{
    public int Id { get; set; }
    public int InspectionId { get; set; }
    public decimal Amount { get; set; }
    public string Reason { get; set; } = string.Empty;
    public bool IsPaid { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public Inspection Inspection { get; set; } = null!;
}