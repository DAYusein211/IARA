using IARA.Application.DTOs.Permit;

namespace IARA.Application.Interfaces;

public interface IPermitService
{
    Task<PermitResponse> CreatePermitAsync(CreatePermitRequest request);
    Task<PermitResponse> GetPermitByIdAsync(int id);
    Task<IEnumerable<PermitResponse>> GetAllPermitsAsync();
    Task<IEnumerable<PermitResponse>> GetPermitsByShipIdAsync(int shipId);
    Task<IEnumerable<PermitResponse>> GetExpiringPermitsAsync(int daysFromNow = 30);
    Task<IEnumerable<PermitResponse>> GetActivePermitsAsync();
    Task<IEnumerable<PermitResponse>> GetExpiredPermitsAsync();
    Task<PermitResponse> UpdatePermitAsync(int id, UpdatePermitRequest request);
    Task DeletePermitAsync(int id);
}