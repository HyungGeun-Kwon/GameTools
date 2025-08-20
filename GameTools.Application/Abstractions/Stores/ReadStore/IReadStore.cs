namespace GameTools.Application.Abstractions.Stores.ReadStore
{
    public interface IReadStore<TEntity, TKey> where TEntity : class
    {
        Task<TEntity?> GetByIdAsync(TKey id, CancellationToken ct);
    }
}
