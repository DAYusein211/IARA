using IARA.Domain.Enums;

namespace IARA.Application.DTOs.Inspection;

public class InspectionResponse
{
    public int Id { get; set; }
    public int InspectorId { get; set; }
    public string InspectorName { get; set; } = string.Empty;
    public TargetType TargetType { get; set; }
    public int TargetId { get; set; }
    public string TargetDescription { get; set; } = string.Empty;
    public DateTime InspectionDate { get; set; }
    public InspectionResult Result { get; set; }
    public string Notes { get; set; } = string.Empty;
    public FineResponse? Fine { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class FineResponse
{
    public int Id { get; set; }
    public int InspectionId { get; set; }
    public decimal Amount { get; set; }
    public string Reason { get; set; } = string.Empty;
    public bool IsPaid { get; set; }
    public DateTime CreatedAt { get; set; }
}