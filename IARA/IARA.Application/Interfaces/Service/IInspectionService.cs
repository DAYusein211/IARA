using IARA.Application.DTOs.Inspection;
using IARA.Domain.Enums;

namespace IARA.Application.Interfaces;

public interface IInspectionService
{
    Task<InspectionResponse> CreateInspectionAsync(CreateInspectionRequest request);
    Task<InspectionResponse> GetInspectionByIdAsync(int id);
    Task<IEnumerable<InspectionResponse>> GetAllInspectionsAsync();
    Task<IEnumerable<InspectionResponse>> GetInspectionsByInspectorIdAsync(int inspectorId);
    Task<IEnumerable<InspectionResponse>> GetInspectionsByTargetAsync(TargetType targetType, int targetId);
    Task<IEnumerable<InspectionResponse>> GetInspectionsByResultAsync(InspectionResult result);
    Task<IEnumerable<InspectionResponse>> GetInspectionsWithFinesAsync();
    Task<IEnumerable<InspectionResponse>> GetInspectionsWithUnpaidFinesAsync();
    Task<InspectionResponse> UpdateInspectionAsync(int id, UpdateInspectionRequest request);
    Task<InspectionResponse> MarkFineAsPaidAsync(int inspectionId);
    Task DeleteInspectionAsync(int id);
}