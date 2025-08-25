using GameTools.Client.Application.Common.Paging;
using GameTools.Client.Application.UseCases.Items.BulkInsertItems;
using GameTools.Client.Application.UseCases.Items.BulkUpdateItems;
using GameTools.Client.Application.UseCases.Items.CreateItem;
using GameTools.Client.Application.UseCases.Items.DeleteItem;
using GameTools.Client.Application.UseCases.Items.GetItemsPage;
using GameTools.Client.Application.UseCases.Items.UpdateItem;
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
        Task<BulkUpdateItemsOutput> BulkUpdateAsync(BulkUpdateItemsInput input, CancellationToken ct);
    }
}
