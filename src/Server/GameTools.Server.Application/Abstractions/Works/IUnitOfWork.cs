namespace GameTools.Server.Application.Abstractions.Works
{
    public interface IUnitOfWork
    {
        Task<int> SaveChangesAsync(CancellationToken ct = default);
    }
}
