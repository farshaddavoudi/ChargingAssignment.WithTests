using CharginAssignment.WithTests.Application.Common.Contracts;
using Microsoft.EntityFrameworkCore.Storage;

namespace CharginAssignment.WithTests.Infrastructure.Persistence.EFCore.UnitOfWork;

public sealed class UnitOfWork(AppDbContext dbContext) : IUnitOfWork
{
    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public void SaveChange()
    {
        dbContext.SaveChanges();
    }

    public async Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken)
    {
        return await dbContext.Database.BeginTransactionAsync(cancellationToken);
    }

    public void ClearChangeTracker()
    {
        dbContext.ChangeTracker.Clear();
    }

    private bool _disposed;

    private void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
                dbContext.Dispose();
        }

        _disposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}