using IARA.Application.DTOs.Inspection;
using IARA.Application.Interfaces;
using IARA.Domain.Entities;
using IARA.Domain.Enums;

namespace IARA.Application.Services;

public class InspectionService : IInspectionService
{
    private readonly IUnitOfWork _unitOfWork;

    public InspectionService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<InspectionResponse> CreateInspectionAsync(CreateInspectionRequest request)
    {
        var inspector = await _unitOfWork.Users.GetByIdAsync(request.InspectorId);
        if (inspector == null || inspector.Role != UserRole.INSPECTOR)
        {
            throw new InvalidOperationException("Invalid inspector");
        }

        await ValidateTargetExistsAsync(request.TargetType, request.TargetId);

        var inspection = new Inspection
        {
            InspectorId = request.InspectorId,
            TargetType = request.TargetType,
            TargetId = request.TargetId,
            InspectionDate = request.InspectionDate,
            Result = request.Result,
            Notes = request.Notes,
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Inspections.AddAsync(inspection);
        await _unitOfWork.SaveChangesAsync();

        if (request.Result == InspectionResult.FAILED && request.Fine != null)
        {
            var fine = new Fine
            {
                InspectionId = inspection.Id,
                Amount = request.Fine.Amount,
                Reason = request.Fine.Reason,
                IsPaid = false,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Fines.AddAsync(fine);
            await _unitOfWork.SaveChangesAsync();
        }

        var createdInspection = await _unitOfWork.Inspections.GetByIdWithDetailsAsync(inspection.Id);
        return await MapToResponseAsync(createdInspection!);
    }

    public async Task<InspectionResponse> GetInspectionByIdAsync(int id)
    {
        var inspection = await _unitOfWork.Inspections.GetByIdWithDetailsAsync(id);
        if (inspection == null)
        {
            throw new InvalidOperationException("Inspection not found");
        }

        return await MapToResponseAsync(inspection);
    }

    public async Task<IEnumerable<InspectionResponse>> GetAllInspectionsAsync()
    {
        var inspections = await _unitOfWork.Inspections.GetAllWithDetailsAsync();
        var responses = new List<InspectionResponse>();
        
        foreach (var inspection in inspections)
        {
            responses.Add(await MapToResponseAsync(inspection));
        }
        
        return responses;
    }

    public async Task<IEnumerable<InspectionResponse>> GetInspectionsByInspectorIdAsync(int inspectorId)
    {
        var inspections = await _unitOfWork.Inspections.GetByInspectorIdAsync(inspectorId);
        var responses = new List<InspectionResponse>();
        
        foreach (var inspection in inspections)
        {
            responses.Add(await MapToResponseAsync(inspection));
        }
        
        return responses;
    }

    public async Task<IEnumerable<InspectionResponse>> GetInspectionsByTargetAsync(TargetType targetType, int targetId)
    {
        var inspections = await _unitOfWork.Inspections.GetByTargetAsync(targetType, targetId);
        var responses = new List<InspectionResponse>();
        
        foreach (var inspection in inspections)
        {
            responses.Add(await MapToResponseAsync(inspection));
        }
        
        return responses;
    }

    public async Task<IEnumerable<InspectionResponse>> GetInspectionsByResultAsync(InspectionResult result)
    {
        var inspections = await _unitOfWork.Inspections.GetByResultAsync(result);
        var responses = new List<InspectionResponse>();
        
        foreach (var inspection in inspections)
        {
            responses.Add(await MapToResponseAsync(inspection));
        }
        
        return responses;
    }

    public async Task<IEnumerable<InspectionResponse>> GetInspectionsWithFinesAsync()
    {
        var inspections = await _unitOfWork.Inspections.GetInspectionsWithFinesAsync();
        var responses = new List<InspectionResponse>();
        
        foreach (var inspection in inspections)
        {
            responses.Add(await MapToResponseAsync(inspection));
        }
        
        return responses;
    }

    public async Task<IEnumerable<InspectionResponse>> GetInspectionsWithUnpaidFinesAsync()
    {
        var inspections = await _unitOfWork.Inspections.GetInspectionsWithUnpaidFinesAsync();
        var responses = new List<InspectionResponse>();
        
        foreach (var inspection in inspections)
        {
            responses.Add(await MapToResponseAsync(inspection));
        }
        
        return responses;
    }

    public async Task<InspectionResponse> UpdateInspectionAsync(int id, UpdateInspectionRequest request)
    {
        var inspection = await _unitOfWork.Inspections.GetByIdWithDetailsAsync(id);
        if (inspection == null)
        {
            throw new InvalidOperationException("Inspection not found");
        }

        inspection.Result = request.Result;
        inspection.Notes = request.Notes;

        if (request.Result == InspectionResult.FAILED && request.Fine != null)
        {
            if (inspection.Fine != null)
            {
                inspection.Fine.Amount = request.Fine.Amount;
                inspection.Fine.Reason = request.Fine.Reason;
                _unitOfWork.Fines.Update(inspection.Fine);
            }
            else
            {
                var fine = new Fine
                {
                    InspectionId = inspection.Id,
                    Amount = request.Fine.Amount,
                    Reason = request.Fine.Reason,
                    IsPaid = false,
                    CreatedAt = DateTime.UtcNow
                };
                await _unitOfWork.Fines.AddAsync(fine);
            }
        }
        else if (inspection.Fine != null)
        {
            // Remove fine if result is no longer FAILED
            _unitOfWork.Fines.Delete(inspection.Fine);
        }

        _unitOfWork.Inspections.Update(inspection);
        await _unitOfWork.SaveChangesAsync();

        var updatedInspection = await _unitOfWork.Inspections.GetByIdWithDetailsAsync(id);
        return await MapToResponseAsync(updatedInspection!);
    }

    public async Task<InspectionResponse> MarkFineAsPaidAsync(int inspectionId)
    {
        var inspection = await _unitOfWork.Inspections.GetByIdWithDetailsAsync(inspectionId);
        if (inspection == null)
        {
            throw new InvalidOperationException("Inspection not found");
        }

        if (inspection.Fine == null)
        {
            throw new InvalidOperationException("No fine found for this inspection");
        }

        inspection.Fine.IsPaid = true;
        _unitOfWork.Fines.Update(inspection.Fine);
        await _unitOfWork.SaveChangesAsync();

        return await MapToResponseAsync(inspection);
    }

    public async Task DeleteInspectionAsync(int id)
    {
        var inspection = await _unitOfWork.Inspections.GetByIdAsync(id);
        if (inspection == null)
        {
            throw new InvalidOperationException("Inspection not found");
        }

        _unitOfWork.Inspections.Delete(inspection);
        await _unitOfWork.SaveChangesAsync();
    }

    private async Task ValidateTargetExistsAsync(TargetType targetType, int targetId)
    {
        switch (targetType)
        {
            case TargetType.SHIP:
                var ship = await _unitOfWork.Ships.GetByIdAsync(targetId);
                if (ship == null) throw new InvalidOperationException("Ship not found");
                break;
            case TargetType.FISHER:
                var fisher = await _unitOfWork.Users.GetByIdAsync(targetId);
                if (fisher == null) throw new InvalidOperationException("Fisher not found");
                break;
            case TargetType.FISHING_TRIP:
                var trip = await _unitOfWork.FishingTrips.GetByIdAsync(targetId);
                if (trip == null) throw new InvalidOperationException("Fishing trip not found");
                break;
        }
    }

    private async Task<string> GetTargetDescriptionAsync(TargetType targetType, int targetId)
    {
        switch (targetType)
        {
            case TargetType.SHIP:
                var ship = await _unitOfWork.Ships.GetByIdAsync(targetId);
                return ship != null ? $"Ship: {ship.Name} ({ship.RegistrationNumber})" : "Unknown Ship";
            case TargetType.FISHER:
                var fisher = await _unitOfWork.Users.GetByIdAsync(targetId);
                return fisher != null ? $"Fisher: {fisher.FullName}" : "Unknown Fisher";
            case TargetType.FISHING_TRIP:
                var trip = await _unitOfWork.FishingTrips.GetByIdAsync(targetId);
                return trip != null ? $"Fishing Trip #{trip.Id}" : "Unknown Trip";
            default:
                return "Unknown Target";
        }
    }

    private async Task<InspectionResponse> MapToResponseAsync(Inspection inspection)
    {
        var targetDescription = await GetTargetDescriptionAsync(inspection.TargetType, inspection.TargetId);

        return new InspectionResponse
        {
            Id = inspection.Id,
            InspectorId = inspection.InspectorId,
            InspectorName = inspection.Inspector.FullName,
            TargetType = inspection.TargetType,
            TargetId = inspection.TargetId,
            TargetDescription = targetDescription,
            InspectionDate = inspection.InspectionDate,
            Result = inspection.Result,
            Notes = inspection.Notes,
            Fine = inspection.Fine != null ? new FineResponse
            {
                Id = inspection.Fine.Id,
                InspectionId = inspection.Fine.InspectionId,
                Amount = inspection.Fine.Amount,
                Reason = inspection.Fine.Reason,
                IsPaid = inspection.Fine.IsPaid,
                CreatedAt = inspection.Fine.CreatedAt
            } : null,
            CreatedAt = inspection.CreatedAt
        };
    }
}