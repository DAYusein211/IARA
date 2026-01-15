using IARA.Application.DTOs.Ship;

namespace IARA.Application.Interfaces;

public interface IShipService
{
    Task<ShipResponse> CreateShipAsync(CreateShipRequest request);
    Task<ShipResponse> GetShipByIdAsync(int id);
    Task<IEnumerable<ShipResponse>> GetAllShipsAsync();
    Task<IEnumerable<ShipResponse>> GetShipsByOwnerIdAsync(int ownerId);
    Task<ShipResponse> UpdateShipAsync(int id, UpdateShipRequest request);
    Task DeleteShipAsync(int id);
}