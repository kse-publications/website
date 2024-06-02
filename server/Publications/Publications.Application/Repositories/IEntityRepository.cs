﻿
using Publications.Domain.Shared;

namespace Publications.Application.Repositories;

public interface IEntityRepository<TEntity>
    where TEntity : Entity<TEntity>
{
    Task<TEntity?> GetByIdAsync(
        int resourceId,
        CancellationToken cancellationToken = default);
    
    Task InsertOrUpdateAsync(
        IEnumerable<TEntity> entities,
        CancellationToken cancellationToken = default);
    
    Task SynchronizeAsync(
        DateTime lastSyncDateTime,
        CancellationToken cancellationToken = default);
}