using IARA.Application.DTOs.FishingTrip;
using IARA.Application.Interfaces;
using IARA.Domain.Entities;

namespace IARA.Application.Services;

public class FishingTripService : IFishingTripService
{
    private readonly IUnitOfWork _unitOfWork;

    public FishingTripService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<FishingTripResponse> CreateFishingTripAsync(CreateFishingTripRequest request)
    {
        if (request.EndTime.HasValue && request.StartTime >= request.EndTime.Value)
        {
            throw new InvalidOperationException("StartTime must be before EndTime");
        }

        var ship = await _unitOfWork.Ships.GetByIdAsync(request.ShipId);
        if (ship == null)
        {
            throw new InvalidOperationException("Ship not found");
        }

        var fishingTrip = new FishingTrip
        {
            ShipId = request.ShipId,
            StartTime = request.StartTime,
            EndTime = request.EndTime,
            FuelUsed = request.FuelUsed,
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.FishingTrips.AddAsync(fishingTrip);
        await _unitOfWork.SaveChangesAsync();

        if (request.Catches.Any())
        {
            foreach (var catchDto in request.Catches)
            {
                var catchEntity = new Catch
                {
                    FishingTripId = fishingTrip.Id,
                    FishType = catchDto.FishType,
                    QuantityKg = catchDto.QuantityKg,
                    CreatedAt = DateTime.UtcNow
                };
                await _unitOfWork.Catches.AddAsync(catchEntity);
            }
            await _unitOfWork.SaveChangesAsync();
        }

        // Reload with full details
        var createdTrip = await _unitOfWork.FishingTrips.GetByIdWithDetailsAsync(fishingTrip.Id);
        return MapToResponse(createdTrip!);
    }

    public async Task<FishingTripResponse> GetFishingTripByIdAsync(int id)
    {
        var trip = await _unitOfWork.FishingTrips.GetByIdWithDetailsAsync(id);
        if (trip == null)
        {
            throw new InvalidOperationException("Fishing trip not found");
        }

        return MapToResponse(trip);
    }

    public async Task<IEnumerable<FishingTripResponse>> GetAllFishingTripsAsync()
    {
        var trips = await _unitOfWork.FishingTrips.GetAllWithDetailsAsync();
        return trips.Select(MapToResponse);
    }

    public async Task<IEnumerable<FishingTripResponse>> GetFishingTripsByShipIdAsync(int shipId)
    {
        var trips = await _unitOfWork.FishingTrips.GetByShipIdWithDetailsAsync(shipId);
        return trips.Select(MapToResponse);
    }

    public async Task<IEnumerable<FishingTripResponse>> GetActiveFishingTripsAsync()
    {
        var trips = await _unitOfWork.FishingTrips.GetActiveTripsAsync();
        return trips.Select(MapToResponse);
    }

    public async Task<IEnumerable<FishingTripResponse>> GetCompletedFishingTripsAsync()
    {
        var trips = await _unitOfWork.FishingTrips.GetCompletedTripsAsync();
        return trips.Select(MapToResponse);
    }

    public async Task<FishingTripResponse> UpdateFishingTripAsync(int id, UpdateFishingTripRequest request)
    {
        var trip = await _unitOfWork.FishingTrips.GetByIdWithDetailsAsync(id);
        if (trip == null)
        {
            throw new InvalidOperationException("Fishing trip not found");
        }

        if (request.EndTime.HasValue && trip.StartTime >= request.EndTime.Value)
        {
            throw new InvalidOperationException("EndTime must be after StartTime");
        }

        trip.EndTime = request.EndTime;
        trip.FuelUsed = request.FuelUsed;

        var existingCatches = trip.Catches.ToList();
        foreach (var existingCatch in existingCatches)
        {
            _unitOfWork.Catches.Delete(existingCatch);
        }

        if (request.Catches.Any())
        {
            foreach (var catchDto in request.Catches)
            {
                var catchEntity = new Catch
                {
                    FishingTripId = trip.Id,
                    FishType = catchDto.FishType,
                    QuantityKg = catchDto.QuantityKg,
                    CreatedAt = DateTime.UtcNow
                };
                await _unitOfWork.Catches.AddAsync(catchEntity);
            }
        }

        _unitOfWork.FishingTrips.Update(trip);
        await _unitOfWork.SaveChangesAsync();

        var updatedTrip = await _unitOfWork.FishingTrips.GetByIdWithDetailsAsync(id);
        return MapToResponse(updatedTrip!);
    }

    public async Task<FishingTripResponse> CompleteFishingTripAsync(int id, decimal? fuelUsed)
    {
        var trip = await _unitOfWork.FishingTrips.GetByIdWithDetailsAsync(id);
        if (trip == null)
        {
            throw new InvalidOperationException("Fishing trip not found");
        }

        if (trip.EndTime.HasValue)
        {
            throw new InvalidOperationException("Fishing trip is already completed");
        }

        trip.EndTime = DateTime.UtcNow;
        trip.FuelUsed = fuelUsed;

        _unitOfWork.FishingTrips.Update(trip);
        await _unitOfWork.SaveChangesAsync();

        return MapToResponse(trip);
    }

    public async Task DeleteFishingTripAsync(int id)
    {
        var trip = await _unitOfWork.FishingTrips.GetByIdAsync(id);
        if (trip == null)
        {
            throw new InvalidOperationException("Fishing trip not found");
        }

        _unitOfWork.FishingTrips.Delete(trip);
        await _unitOfWork.SaveChangesAsync();
    }

    private FishingTripResponse MapToResponse(FishingTrip trip)
    {
        double? durationHours = null;
        if (trip.EndTime.HasValue)
        {
            durationHours = (trip.EndTime.Value - trip.StartTime).TotalHours;
        }

        var totalCatch = trip.Catches.Sum(c => c.QuantityKg);

        return new FishingTripResponse
        {
            Id = trip.Id,
            ShipId = trip.ShipId,
            ShipName = trip.Ship.Name,
            ShipRegistrationNumber = trip.Ship.RegistrationNumber,
            StartTime = trip.StartTime,
            EndTime = trip.EndTime,
            FuelUsed = trip.FuelUsed,
            IsCompleted = trip.EndTime.HasValue,
            DurationHours = durationHours,
            TotalCatchKg = totalCatch,
            Catches = trip.Catches.Select(c => new CatchResponse
            {
                Id = c.Id,
                FishingTripId = c.FishingTripId,
                FishType = c.FishType,
                QuantityKg = c.QuantityKg,
                CreatedAt = c.CreatedAt
            }).ToList(),
            CreatedAt = trip.CreatedAt
        };
    }
}