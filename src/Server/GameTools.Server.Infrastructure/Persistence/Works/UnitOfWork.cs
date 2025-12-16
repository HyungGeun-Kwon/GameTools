using GameTools.Server.Application.Abstractions.UnitOfWorks;
using Microsoft.EntityFrameworkCore.Storage;

namespace GameTools.Server.Infrastructure.Persistence.Works
{
    public sealed class UnitOfWork(AppDbContext db) : IUnitOfWork
    {
        public Task<int> SaveChangesAsync(CancellationToken ct = default)
            => db.SaveChangesAsync(ct);


        public async Task<IUnitOfWorkTransaction> BeginTransactionAsync(CancellationToken ct = default)
        {
            var tx = await db.Database.BeginTransactionAsync(ct);
            return new EfUowTransaction(tx);
        }

        private sealed class EfUowTransaction(IDbContextTransaction tx) : IUnitOfWorkTransaction
        {
            public Task CommitAsync(CancellationToken ct = default) => tx.CommitAsync(ct);
            public Task RollbackAsync(CancellationToken ct = default) => tx.RollbackAsync(ct);
            public ValueTask DisposeAsync() => tx.DisposeAsync();
        }
    }
}