using GameTools.Client.Application.Common.Paging;
using GameTools.Client.Application.UseCases.Items.BulkDeleteItems;
using GameTools.Client.Application.UseCases.Items.BulkInsertItems;
using GameTools.Client.Application.UseCases.Items.BulkUpdateItems;
using GameTools.Client.Application.UseCases.Items.CreateItem;
using GameTools.Client.Application.UseCases.Items.GetItemsPage;
using GameTools.Client.Application.UseCases.Items.RestoreItemsAsOf;
using GameTools.Client.Application.UseCases.Items.UpdateItem;
using GameTools.Client.Domain.Items;
using GameTools.Contracts.Common;
using GameTools.Contracts.Items.BulkDeleteItems;
using GameTools.Contracts.Items.BulkInsertItems;
using GameTools.Contracts.Items.BulkUpdateItems;
using GameTools.Contracts.Items.Common;
using GameTools.Contracts.Items.CreateItem;
using GameTools.Contracts.Items.GetItemPage;
using GameTools.Contracts.Items.GetItemsByRarity;
using GameTools.Contracts.Items.RestoreItemsAsOf;
using GameTools.Contracts.Items.UpdateItem;

namespace GameTools.Client.Infrastructure.Mapper
{
    public static class ItemsContractMapper
    {
        public static ItemsPageRequest ToContract(this GetItemsPageInput input)
            => new(
                input.Pagination.PageNumber,
                input.Pagination.PageSize,
                input.Filter?.NameSearch,
                input.Filter?.RarityId
            );

        public static CreateItemRequest ToContract(this CreateItemInput input)
            => new(input.Name, input.Price, input.RarityId, input.Description);
        
        public static UpdateItemRequest ToContract(this UpdateItemInput input)
            => new(input.Id, input.Name, input.Price, input.Description, input.RarityId, input.RowVersionBase64);

        public static BulkInsertItemsRequest ToContract(this BulkInsertItemsInput input)
            => new(input.Inputs.Select(i => new BulkInsertItemRow(i.Name, i.Price, i.RarityId, i.Description)).ToList());


        public static BulkUpdateItemsRequest ToContract(this BulkUpdateItemsInput input)
            => new(input.Inputs.Select(i => new BulkUpdateItemRow(i.Id, i.Name, i.Price, i.Description, i.RarityId, i.RowVersionBase64)).ToList());

        public static BulkDeleteItemsRequest ToContract(this BulkDeleteItemsInput input)
            => new(input.Inputs.Select(i => new BulkDeleteItemRow(i.Id, i.RowVersionBase64)).ToList());

        public static RestoreItemsAsOfRequest ToContract(this RestoreItemsAsOfInput input)
            => new(input.AsOfUtc, input.ItemId, input.DryRun, input.Notes);

        public static Item ToClient(this ItemResponse r) 
            => new(
                r.Id, r.Name, r.Price, r.Description, 
                r.RarityId, r.RarityGrade, r.RarityColorCode, 
                r.RowVersionBase64
            );

        public static PagedOutput<Item> ToClient(this PagedResponse<ItemResponse> p) 
            => new(p.Items.Select(ToClient).ToList(), p.TotalCount, p.PageNumber, p.PageSize);

        public static IReadOnlyList<Item> ToClient(this ItemsByRarityResponse resp) 
            => resp.Items.Select(ToClient).ToList();

        public static BulkInsertItemOutputRow ToClient(this BulkInsertItemResult result)
            => new(result.Id, result.Status, result.RowVersionBase64);

        public static BulkInsertItemsOutput ToClient(this BulkInsertItemsResponse resp)
            => new(resp.Results.Select(ToClient).ToList());

        public static BulkUpdateItemOutputRow ToClient(this BulkUpdateItemResult result)
            => new(result.Id, result.Status, result.NewRowVersionBase64);

        public static BulkUpdateItemsOutput ToClient(this BulkUpdateItemsResponse resp)
            => new(resp.Results.Select(ToClient).ToList());

        public static BulkDeleteItemOutputRow ToClient(this BulkDeleteItemResult result)
            => new(result.Id, result.Status);

        public static BulkDeleteItemsOutput ToClient(this BulkDeleteItemsResponse resp)
            => new(resp.Results.Select(ToClient).ToList());

        public static RestoreItemsAsOfOutput ToClient(this RestoreItemsAsOfResponse resp)
            => new(resp.RestoreId, resp.Deleted, resp.Inserted, resp.Updated, resp.IsChanged);
    }
}
