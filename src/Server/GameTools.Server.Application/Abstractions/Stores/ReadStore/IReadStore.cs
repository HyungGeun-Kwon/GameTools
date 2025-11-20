namespace GameTools.Server.Application.Abstractions.Stores.ReadStore
{
    public interface IReadStore<TReadModel, TKey> where TReadModel : class
    {
        Task<TReadModel?> GetByIdAsync(TKey id, CancellationToken ct);
    }
}
