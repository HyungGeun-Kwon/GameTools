using GameTools.Server.Application.Common.Paging;
using GameTools.Server.Application.Features.Items.Commands.CreateItem;
using GameTools.Server.Application.Features.Items.Commands.DeleteItem;
using GameTools.Server.Application.Features.Items.Commands.InsertItemsTvp;
using GameTools.Server.Application.Features.Items.Commands.UpdateItem;
using GameTools.Server.Application.Features.Items.Commands.UpdateItemsTvp;
using GameTools.Server.Application.Features.Items.Models;
using GameTools.Server.Application.Features.Items.Queries.GetItemPage;
using GameTools.Contracts.Items.BulkUpdateItems;
using GameTools.Contracts.Items.Common;
using GameTools.Contracts.Items.CreateItem;
using GameTools.Contracts.Items.GetItemPage;
using GameTools.Contracts.Items.GetItemsByRarity;
using GameTools.Contracts.Items.UpdateItem;
using GameTools.Server.Api.Extensions;
using GameTools.Contracts.Common;
using GameTools.Contracts.Items.BulkInsertItems;

namespace GameTools.Server.Api.Mapper
{
    internal static class ItemApiMapper
    {
        internal static ItemsPageSearchCriteria ToCriteria(this ItemsPageRequest req)
            => new(new Pagination(req.PageNumber, req.PageSize), new ItemsFilter(req.NameSearch, req.RarityId));

        internal static ItemResponse ToResponse(this ItemReadModel readModel)
            => new
            (
                readModel.Id, readModel.Name, readModel.Price, readModel.Description,
                readModel.RarityId, readModel.RarityGrade, readModel.RarityColorCode,
                readModel.RowVersion.ToBase64RowVersion()
            );

        internal static PagedResponse<ItemResponse> ToResponse(this PagedResult<ItemReadModel> pagedResult)
            => new
            (
                pagedResult.Items.Select(ToResponse).ToList(),
                pagedResult.TotalCount, 
                pagedResult.PageNumber, 
                pagedResult.PageSize
            );

        internal static CreateItemPayload ToPayload(this CreateItemRequest req)
            => new(req.Name, req.Price, req.RarityId, req.Description);

        internal static UpdateItemPayload ToPayload(this UpdateItemRequest req)
            => new
            (
                req.Id, req.Name, req.Price, req.Description, 
                req.RarityId, req.RowVersionBase64.FromBase64RowVersion()
            );

        internal static IReadOnlyList<InsertItemRow> ToRows(this BulkInsertItemsRequest req)
            => req.BulkInsertItems.Select(
                x => new InsertItemRow(x.Name, x.Price, x.RarityId, x.Description)).ToList();

        internal static IReadOnlyList<UpdateItemRow> ToRows(this BulkUpdateItemsRequest req)
            => req.BulkUpdateItemRows.Select(
                x => new UpdateItemRow(
                    x.Id, x.Name, x.Price, x.Description, 
                    x.RarityId, x.RowVersionBase64.FromBase64RowVersion())).ToList();

        internal static BulkInsertItemsResponse ToResponse(this IReadOnlyList<InsertItemResultRow> rows)
            => new(rows.Select(r => new BulkInsertItemResult(r.Id, r.RowVersion.ToBase64RowVersion())).ToList());

        internal static BulkUpdateItemsResponse ToResponse(this IReadOnlyList<UpdateItemResultRow> rows)
            => new(rows.Select(r => new BulkUpdateItemResult(r.Id, r.StatusCode.ToString(), r.NewRowVersion?.ToBase64RowVersion())).ToList());

        internal static ItemsByRarityResponse ToResponse(this IReadOnlyList<ItemReadModel> rows)
            => new(rows.Select(
                r => new ItemResponse
                (
                    r.Id, r.Name, r.Price, r.Description,
                    r.RarityId, r.RarityGrade, r.RarityColorCode, 
                    r.RowVersion.ToBase64RowVersion()
                )).ToList());
    }
}
