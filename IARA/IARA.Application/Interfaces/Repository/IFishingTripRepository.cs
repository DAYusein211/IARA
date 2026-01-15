using IARA.Domain.Entities;

namespace IARA.Application.Interfaces;

public interface IFishingTripRepository : IRepository<FishingTrip>
{
    Task<FishingTrip?> GetByIdWithDetailsAsync(int id);
    Task<IEnumerable<FishingTrip>> GetAllWithDetailsAsync();
    Task<IEnumerable<FishingTrip>> GetByShipIdWithDetailsAsync(int shipId);
    Task<IEnumerable<FishingTrip>> GetActiveTripsAsync();
    Task<IEnumerable<FishingTrip>> GetCompletedTripsAsync();
    Task<IEnumerable<FishingTrip>> GetTripsByDateRangeAsync(DateTime from, DateTime to);
}