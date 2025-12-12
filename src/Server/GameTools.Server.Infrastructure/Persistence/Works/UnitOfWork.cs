using GameTools.Server.Application.Abstractions.UnitOfWorks;

namespace GameTools.Server.Infrastructure.Persistence.Works
{
    public sealed class UnitOfWork(AppDbContext db) : IUnitOfWork
    {
        public Task<int> SaveChangesAsync(CancellationToken ct = default)
            => db.SaveChangesAsync(ct);
    }
}