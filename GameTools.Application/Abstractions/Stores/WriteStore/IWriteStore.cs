namespace GameTools.Application.Abstractions.Stores.WriteStore
{
    public interface IWriteStore<TEntity, TKey> where TEntity : class
    {
        Task<TEntity?> GetByIdAsync(TKey id, CancellationToken ct);
        Task AddAsync(TEntity entity, CancellationToken ct);
        void Remove(TEntity entity);
    }
}
