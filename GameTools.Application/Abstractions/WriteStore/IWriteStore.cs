using GameTools.Domain.Entities;

namespace GameTools.Application.Abstractions.WriteStore
{
    public interface IWriteStore<TEntity, TKey> where TEntity : class
    {
        Task<TEntity?> GetByIdAsync(TKey id, CancellationToken ct);
        Task AddAsync(TEntity entity, CancellationToken ct);
        void Remove(TEntity entity);
    }
}
