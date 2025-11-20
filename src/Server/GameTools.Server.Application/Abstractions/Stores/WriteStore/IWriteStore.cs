namespace GameTools.Server.Application.Abstractions.Stores.WriteStore
{
    public interface IWriteStore<TAggregateRoot, TKey> where TAggregateRoot : class
    {
        Task<TAggregateRoot?> LoadForUpdateAsync(TKey id, CancellationToken ct);
        Task AddAsync(TAggregateRoot aggregate, CancellationToken ct);
        void Remove(TAggregateRoot aggregate);
        void SetOriginalRowVersion(TAggregateRoot aggregate, byte[] rowVersion);
        byte[] GetRowVersion(TAggregateRoot aggregate);
    }
}
