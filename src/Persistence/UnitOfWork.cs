using System.Data;
using Domain.Primitives;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Persistence;

internal sealed class UnitOfWork(ApplicationDbContext dbContext) : IUnitOfWork
{
    #region Implementations
    
    public IDbTransaction BeginTransaction()
    {
        var transaction = dbContext.Database.BeginTransaction();
        return transaction.GetDbTransaction();
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateAuditableEntities();
        return dbContext.SaveChangesAsync(cancellationToken);
    }
    
    #endregion

    #region Private Methods

    /// <summary> 
    /// Updates auditable entities with creation and modification timestamps. 
    /// </summary>
    private void UpdateAuditableEntities()
    {
        var entries =
           dbContext
              .ChangeTracker
              .Entries<IAuditableEntity>();
        
        foreach (var entityEntry in entries)
        {
            if (entityEntry.State == EntityState.Added)
            {
                entityEntry.Property(a => a.CreatedOnUtc).CurrentValue = DateTime.UtcNow;
            }
            if (entityEntry.State == EntityState.Modified)
            {
                entityEntry.Property(a => a.ModifiedOnUtc).CurrentValue = DateTime.UtcNow;
            }
        }
    }
    
    #endregion
}
