using Microsoft.EntityFrameworkCore.Storage;
using IARA.Application.Interfaces;
using IARA.Domain.Entities;
using IARA.Infrastructure.Data;

namespace IARA.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly IaraDbContext _context;
    private IDbContextTransaction? _transaction;

    public UnitOfWork(IaraDbContext context)
    {
        _context = context;
        Users = new Repository<User>(_context);
        Ships = new ShipRepository(_context);
        Permits = new PermitRepository(_context);
        FishingTrips = new FishingTripRepository(_context);
        Catches = new Repository<Catch>(_context);
        Tickets = new TicketRepository(_context);
        Inspections = new InspectionRepository(_context);
        Fines = new Repository<Fine>(_context);
    }

    public IRepository<User> Users { get; }
    public IShipRepository Ships { get; }
    public IPermitRepository Permits { get; }
    public IFishingTripRepository FishingTrips { get; }
    public IRepository<Catch> Catches { get; }
    public ITicketRepository Tickets { get; }
    public IInspectionRepository Inspections { get; }
    public IRepository<Fine> Fines { get; }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public async Task BeginTransactionAsync()
    {
        _transaction = await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.CommitAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task RollbackTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}