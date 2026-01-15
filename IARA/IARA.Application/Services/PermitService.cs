using IARA.Application.DTOs.Permit;
using IARA.Application.Interfaces;
using IARA.Domain.Entities;

namespace IARA.Application.Services;

public class PermitService : IPermitService
{
    private readonly IUnitOfWork _unitOfWork;

    public PermitService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PermitResponse> CreatePermitAsync(CreatePermitRequest request)
    {
  
        if (request.ValidFrom >= request.ValidTo)
        {
            throw new InvalidOperationException("Permit end date must be after start date.");
        }


        var ship = await _unitOfWork.Ships.GetByIdAsync(request.ShipId);
        if (ship == null)
        {
            throw new InvalidOperationException("Ship not found");
        }

        var permit = new Permit
        {
            ShipId = request.ShipId,
            ValidFrom = request.ValidFrom,
            ValidTo = request.ValidTo,
            AllowedGear = request.AllowedGear,
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Permits.AddAsync(permit);
        await _unitOfWork.SaveChangesAsync();


        var createdPermit = await _unitOfWork.Permits.GetByIdWithShipAsync(permit.Id);
        return MapToResponse(createdPermit!);
    }

    public async Task<PermitResponse> GetPermitByIdAsync(int id)
    {
        var permit = await _unitOfWork.Permits.GetByIdWithShipAsync(id);
        if (permit == null)
        {
            throw new InvalidOperationException("Permit not found");
        }

        return MapToResponse(permit);
    }

    public async Task<IEnumerable<PermitResponse>> GetAllPermitsAsync()
    {
        var permits = await _unitOfWork.Permits.GetAllWithShipsAsync();
        return permits.Select(MapToResponse);
    }

    public async Task<IEnumerable<PermitResponse>> GetPermitsByShipIdAsync(int shipId)
    {
        var permits = await _unitOfWork.Permits.GetByShipIdWithShipAsync(shipId);
        return permits.Select(MapToResponse);
    }

    public async Task<IEnumerable<PermitResponse>> GetExpiringPermitsAsync(int daysFromNow = 30)
    {
        var permits = await _unitOfWork.Permits.GetExpiringPermitsAsync(daysFromNow);
        return permits.Select(MapToResponse);
    }

    public async Task<IEnumerable<PermitResponse>> GetActivePermitsAsync()
    {
        var permits = await _unitOfWork.Permits.GetActivePermitsAsync();
        return permits.Select(MapToResponse);
    }

    public async Task<IEnumerable<PermitResponse>> GetExpiredPermitsAsync()
    {
        var permits = await _unitOfWork.Permits.GetExpiredPermitsAsync();
        return permits.Select(MapToResponse);
    }

    public async Task<PermitResponse> UpdatePermitAsync(int id, UpdatePermitRequest request)
    {
        var permit = await _unitOfWork.Permits.GetByIdWithShipAsync(id);
        if (permit == null)
        {
            throw new InvalidOperationException("Permit not found");
        }

   
        if (request.ValidFrom >= request.ValidTo)
        {
            throw new InvalidOperationException("Permit end date must be after start date.");
        }

        permit.ValidFrom = request.ValidFrom;
        permit.ValidTo = request.ValidTo;
        permit.AllowedGear = request.AllowedGear;

        _unitOfWork.Permits.Update(permit);
        await _unitOfWork.SaveChangesAsync();

        return MapToResponse(permit);
    }

    public async Task DeletePermitAsync(int id)
    {
        var permit = await _unitOfWork.Permits.GetByIdAsync(id);
        if (permit == null)
        {
            throw new InvalidOperationException("Permit not found");
        }

        _unitOfWork.Permits.Delete(permit);
        await _unitOfWork.SaveChangesAsync();
    }

    private PermitResponse MapToResponse(Permit permit)
    {
        var now = DateTime.UtcNow;
        var daysUntilExpiry = (permit.ValidTo - now).Days;

        return new PermitResponse
        {
            Id = permit.Id,
            ShipId = permit.ShipId,
            ShipName = permit.Ship.Name,
            ShipRegistrationNumber = permit.Ship.RegistrationNumber,
            ValidFrom = permit.ValidFrom,
            ValidTo = permit.ValidTo,
            AllowedGear = permit.AllowedGear,
            IsExpired = permit.ValidTo < now,
            DaysUntilExpiry = daysUntilExpiry > 0 ? daysUntilExpiry : 0,
            CreatedAt = permit.CreatedAt
        };
    }
}