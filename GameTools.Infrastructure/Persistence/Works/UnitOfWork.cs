using GameTools.Application.Abstractions.Works;

namespace GameTools.Infrastructure.Persistence.Works
{
    public sealed class UnitOfWork(AppDbContext db) : IUnitOfWork
    {
        public Task<int> SaveChangesAsync(CancellationToken ct = default)
            => db.SaveChangesAsync(ct);
    }
}
