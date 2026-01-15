using IARA.Application.DTOs.FishingTrip;

namespace IARA.Application.Interfaces;

public interface IFishingTripService
{
    Task<FishingTripResponse> CreateFishingTripAsync(CreateFishingTripRequest request);
    Task<FishingTripResponse> GetFishingTripByIdAsync(int id);
    Task<IEnumerable<FishingTripResponse>> GetAllFishingTripsAsync();
    Task<IEnumerable<FishingTripResponse>> GetFishingTripsByShipIdAsync(int shipId);
    Task<IEnumerable<FishingTripResponse>> GetActiveFishingTripsAsync();
    Task<IEnumerable<FishingTripResponse>> GetCompletedFishingTripsAsync();
    Task<FishingTripResponse> UpdateFishingTripAsync(int id, UpdateFishingTripRequest request);
    Task<FishingTripResponse> CompleteFishingTripAsync(int id, decimal? fuelUsed);
    Task DeleteFishingTripAsync(int id);
}