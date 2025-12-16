using GameTools.Server.Application.Catalog.Rarities.Models;

namespace GameTools.Server.Application.Abstractions.Stores.ReadStore
{
    public interface IRarityReadStore : IReadStore<RarityReadModel, Guid>
    {
        Task<IReadOnlyList<RarityReadModel>> GetAllAsync(CancellationToken ct);
    }
}
