using IARA.Domain.Entities;
using IARA.Domain.Enums;

namespace IARA.Application.Interfaces;

public interface IInspectionRepository : IRepository<Inspection>
{
    Task<Inspection?> GetByIdWithDetailsAsync(int id);
    Task<IEnumerable<Inspection>> GetAllWithDetailsAsync();
    Task<IEnumerable<Inspection>> GetByInspectorIdAsync(int inspectorId);
    Task<IEnumerable<Inspection>> GetByTargetAsync(TargetType targetType, int targetId);
    Task<IEnumerable<Inspection>> GetByResultAsync(InspectionResult result);
    Task<IEnumerable<Inspection>> GetInspectionsWithFinesAsync();
    Task<IEnumerable<Inspection>> GetInspectionsWithUnpaidFinesAsync();
}