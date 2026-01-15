using IARA.Application.DTOs.Ship;
using IARA.Application.Interfaces;
using IARA.Domain.Entities;

namespace IARA.Application.Services;

public class ShipService : IShipService
{
    private readonly IUnitOfWork _unitOfWork;

    public ShipService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ShipResponse> CreateShipAsync(CreateShipRequest request)
    {
        // Check if owner exists
        var owner = await _unitOfWork.Users.GetByIdAsync(request.OwnerId);
        if (owner == null)
        {
            throw new InvalidOperationException("Owner not found");
        }

        // Check if registration number is unique
        var isUnique = await _unitOfWork.Ships.IsRegistrationNumberUniqueAsync(request.RegistrationNumber);
        if (!isUnique)
        {
            throw new InvalidOperationException("Registration number already exists");
        }

        var ship = new Ship
        {
            Name = request.Name,
            RegistrationNumber = request.RegistrationNumber,
            OwnerId = request.OwnerId,
            EnginePower = request.EnginePower,
            FuelType = request.FuelType,
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Ships.AddAsync(ship);
        await _unitOfWork.SaveChangesAsync();

        return new ShipResponse
        {
            Id = ship.Id,
            Name = ship.Name,
            RegistrationNumber = ship.RegistrationNumber,
            OwnerId = ship.OwnerId,
            OwnerName = owner.FullName,
            EnginePower = ship.EnginePower,
            FuelType = ship.FuelType,
            CreatedAt = ship.CreatedAt
        };
    }

    public async Task<ShipResponse> GetShipByIdAsync(int id)
    {
        var ship = await _unitOfWork.Ships.GetByIdWithOwnerAsync(id);

        if (ship == null)
        {
            throw new InvalidOperationException("Ship not found");
        }

        return new ShipResponse
        {
            Id = ship.Id,
            Name = ship.Name,
            RegistrationNumber = ship.RegistrationNumber,
            OwnerId = ship.OwnerId,
            OwnerName = ship.Owner.FullName,
            EnginePower = ship.EnginePower,
            FuelType = ship.FuelType,
            CreatedAt = ship.CreatedAt
        };
    }

    public async Task<IEnumerable<ShipResponse>> GetAllShipsAsync()
    {
        var ships = await _unitOfWork.Ships.GetAllWithOwnersAsync();

        return ships.Select(ship => new ShipResponse
        {
            Id = ship.Id,
            Name = ship.Name,
            RegistrationNumber = ship.RegistrationNumber,
            OwnerId = ship.OwnerId,
            OwnerName = ship.Owner.FullName,
            EnginePower = ship.EnginePower,
            FuelType = ship.FuelType,
            CreatedAt = ship.CreatedAt
        });
    }

    public async Task<IEnumerable<ShipResponse>> GetShipsByOwnerIdAsync(int ownerId)
    {
        var ships = await _unitOfWork.Ships.GetByOwnerIdWithOwnerAsync(ownerId);

        return ships.Select(ship => new ShipResponse
        {
            Id = ship.Id,
            Name = ship.Name,
            RegistrationNumber = ship.RegistrationNumber,
            OwnerId = ship.OwnerId,
            OwnerName = ship.Owner.FullName,
            EnginePower = ship.EnginePower,
            FuelType = ship.FuelType,
            CreatedAt = ship.CreatedAt
        });
    }

    public async Task<ShipResponse> UpdateShipAsync(int id, UpdateShipRequest request)
    {
        var ship = await _unitOfWork.Ships.GetByIdWithOwnerAsync(id);

        if (ship == null)
        {
            throw new InvalidOperationException("Ship not found");
        }

        ship.Name = request.Name;
        ship.EnginePower = request.EnginePower;
        ship.FuelType = request.FuelType;

        _unitOfWork.Ships.Update(ship);
        await _unitOfWork.SaveChangesAsync();

        return new ShipResponse
        {
            Id = ship.Id,
            Name = ship.Name,
            RegistrationNumber = ship.RegistrationNumber,
            OwnerId = ship.OwnerId,
            OwnerName = ship.Owner.FullName,
            EnginePower = ship.EnginePower,
            FuelType = ship.FuelType,
            CreatedAt = ship.CreatedAt
        };
    }

    public async Task DeleteShipAsync(int id)
    {
        var ship = await _unitOfWork.Ships.GetByIdAsync(id);
        if (ship == null)
        {
            throw new InvalidOperationException("Ship not found");
        }

        _unitOfWork.Ships.Delete(ship);
        await _unitOfWork.SaveChangesAsync();
    }
}