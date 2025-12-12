using GameTools.Server.Application.Abstractions.Stores.WriteStore;
using GameTools.Server.Application.Abstractions.Users;
using GameTools.Server.Application.Features.Restores.Commands.RestoreItems;

namespace GameTools.Server.Infrastructure.Persistence.Operations.Restores.Stores.WriteStores
{
    public sealed class RestoreItemWriteStore(AppDbContext db, ICurrentUser currentUser) : IRestoreItemWriteStore
    {
        public Task<RestoreItemsStoreResult> RestoreItemsAsOfAsync(RestoreItemsSpec payload, CancellationToken ct)
        {
            throw new NotImplementedException();
        }
    }
}
