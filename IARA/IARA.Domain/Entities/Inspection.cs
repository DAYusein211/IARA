using IARA.Domain.Enums;

namespace IARA.Domain.Entities;

public class Inspection
{
    public int Id { get; set; }
    public int InspectorId { get; set; }
    public TargetType TargetType { get; set; }
    public int TargetId { get; set; }
    public DateTime InspectionDate { get; set; }
    public InspectionResult Result { get; set; }
    public string Notes { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public User Inspector { get; set; } = null!;
    public Fine? Fine { get; set; }
}