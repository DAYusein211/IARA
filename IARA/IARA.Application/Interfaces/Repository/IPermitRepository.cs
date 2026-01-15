using IARA.Domain.Entities;

namespace IARA.Application.Interfaces;

public interface IPermitRepository : IRepository<Permit>
{
    Task<Permit?> GetByIdWithShipAsync(int id);
    Task<IEnumerable<Permit>> GetAllWithShipsAsync();
    Task<IEnumerable<Permit>> GetByShipIdWithShipAsync(int shipId);
    Task<IEnumerable<Permit>> GetExpiringPermitsAsync(int daysFromNow);
    Task<IEnumerable<Permit>> GetActivePermitsAsync();
    Task<IEnumerable<Permit>> GetExpiredPermitsAsync();
}