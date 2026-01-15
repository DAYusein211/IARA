using IARA.Domain.Entities;

namespace IARA.Application.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IRepository<User> Users { get; }
    IShipRepository Ships { get; }
    IPermitRepository Permits { get; }
    IFishingTripRepository FishingTrips { get; }
    IRepository<Catch> Catches { get; }
    ITicketRepository Tickets { get; }
    IInspectionRepository Inspections { get; }
    IRepository<Fine> Fines { get; }
    
    Task<int> SaveChangesAsync();
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}