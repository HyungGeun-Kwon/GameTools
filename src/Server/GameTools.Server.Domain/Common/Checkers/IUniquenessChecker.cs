namespace GameTools.Server.Domain.Common.Checkers
{
    public interface IUniquenessChecker<T>
    {
        Task<bool> ExistsAsync(T name, CancellationToken ct);
    }
}
