using IARA.Domain.Entities;

namespace IARA.Application.Interfaces;

public interface IShipRepository : IRepository<Ship>
{
    Task<Ship?> GetByIdWithOwnerAsync(int id);
    Task<IEnumerable<Ship>> GetAllWithOwnersAsync();
    Task<IEnumerable<Ship>> GetByOwnerIdWithOwnerAsync(int ownerId);
    Task<bool> IsRegistrationNumberUniqueAsync(string registrationNumber, int? excludeShipId = null);
}