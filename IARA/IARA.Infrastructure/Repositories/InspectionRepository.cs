using Microsoft.EntityFrameworkCore;
using IARA.Application.Interfaces;
using IARA.Domain.Entities;
using IARA.Domain.Enums;
using IARA.Infrastructure.Data;

namespace IARA.Infrastructure.Repositories;

public class InspectionRepository : Repository<Inspection>, IInspectionRepository
{
    public InspectionRepository(IaraDbContext context) : base(context)
    {
    }

    public async Task<Inspection?> GetByIdWithDetailsAsync(int id)
    {
        return await _context.Inspections
            .Include(i => i.Inspector)
            .Include(i => i.Fine)
            .FirstOrDefaultAsync(i => i.Id == id);
    }

    public async Task<IEnumerable<Inspection>> GetAllWithDetailsAsync()
    {
        return await _context.Inspections
            .Include(i => i.Inspector)
            .Include(i => i.Fine)
            .OrderByDescending(i => i.InspectionDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Inspection>> GetByInspectorIdAsync(int inspectorId)
    {
        return await _context.Inspections
            .Include(i => i.Inspector)
            .Include(i => i.Fine)
            .Where(i => i.InspectorId == inspectorId)
            .OrderByDescending(i => i.InspectionDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Inspection>> GetByTargetAsync(TargetType targetType, int targetId)
    {
        return await _context.Inspections
            .Include(i => i.Inspector)
            .Include(i => i.Fine)
            .Where(i => i.TargetType == targetType && i.TargetId == targetId)
            .OrderByDescending(i => i.InspectionDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Inspection>> GetByResultAsync(InspectionResult result)
    {
        return await _context.Inspections
            .Include(i => i.Inspector)
            .Include(i => i.Fine)
            .Where(i => i.Result == result)
            .OrderByDescending(i => i.InspectionDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Inspection>> GetInspectionsWithFinesAsync()
    {
        return await _context.Inspections
            .Include(i => i.Inspector)
            .Include(i => i.Fine)
            .Where(i => i.Fine != null)
            .OrderByDescending(i => i.InspectionDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Inspection>> GetInspectionsWithUnpaidFinesAsync()
    {
        return await _context.Inspections
            .Include(i => i.Inspector)
            .Include(i => i.Fine)
            .Where(i => i.Fine != null && !i.Fine.IsPaid)
            .OrderByDescending(i => i.InspectionDate)
            .ToListAsync();
    }
}