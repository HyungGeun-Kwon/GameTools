using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace GameTools.Server.Test.Utils
{
    public sealed class InMemoryRowVersionInterceptor : SaveChangesInterceptor
    {
        private static byte[] NewVersion() => BitConverter.GetBytes(DateTime.UtcNow.Ticks);
        private static void Touch(DbContext ctx)
        {
            if (ctx is null || !ctx.Database.IsInMemory()) return; 
            
            foreach (var e in ctx.ChangeTracker.Entries())
            {
                // 엔티티에 RowVersion 속성이 있을 때만
                var prop = e.Properties.FirstOrDefault(p => p.Metadata.Name == "RowVersion");
                if (prop is null) continue;

                if (e.State == EntityState.Added && prop.CurrentValue is null)
                    prop.CurrentValue = NewVersion();
                else if (e.State == EntityState.Modified)
                    prop.CurrentValue = NewVersion(); // 업데이트 시 새 버전 시뮬레이션
            }
        }
        public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        {
            Touch(eventData.Context!);
            return base.SavingChanges(eventData, result);
        }
        public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken ct = default)
        {
            Touch(eventData.Context!);
            return await base.SavingChangesAsync(eventData, result, ct);
        }
    }
}
