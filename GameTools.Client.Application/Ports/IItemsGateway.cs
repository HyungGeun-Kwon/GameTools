using System;
using GameTools.Client.Application.Common.Paging;
using GameTools.Client.Application.UseCases.BulkInsertItems;
using GameTools.Client.Application.UseCases.CreateItem;
using GameTools.Client.Application.UseCases.DeleteItem;
using GameTools.Client.Application.UseCases.GetItemsPage;
using GameTools.Client.Domain.Items;

namespace GameTools.Client.Application.Ports
{
    public interface IItemsGateway
    {
        Task<Item?> GetByIdAsync(int id, CancellationToken ct);
        Task<IReadOnlyList<Item>> GetByRarityAsync(byte rarityId, CancellationToken ct);
        Task<PagedOutput<Item>> GetPageAsync(GetItemsPageInput input, CancellationToken ct);

        Task<Item> CreateAsync(CreateItemInput input, CancellationToken ct);
        Task<Item> UpdateAsync(UpdateItemInput input, CancellationToken ct);
        Task DeleteAsync(DeleteItemInput input, CancellationToken ct);
        Task<BulkInsertItemsOutput> BulkInsertAsync(BulkInsertItemsInput input, CancellationToken ct);
    }
}
