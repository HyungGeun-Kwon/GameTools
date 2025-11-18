namespace GameTools.Server.Domain.Common.Policies
{
    public interface IUniquenessPolicy<T>
    {
        Task EnsureUniqueAsync(T value, CancellationToken ct = default);
    }
}
