using GameTools.Client.Application.Common.Paging;
using GameTools.Client.Application.UseCases.GetItemsPage;
using GameTools.Client.Domain.Items;
using GameTools.Contracts.Common;
using GameTools.Contracts.Items.Common;
using GameTools.Contracts.Items.GetItemPage;
using GameTools.Contracts.Items.GetItemsByRarity;

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

        public static Item ToDomain(this ItemResponse r) =>
            new(
                r.Id, r.Name, r.Price, r.Description, 
                r.RarityId, r.RarityGrade, r.RarityColorCode, 
                r.RowVersionBase64
            );

        public static PagedResult<Item> ToDomain(this PagedResponse<ItemResponse> p) =>
            new(p.Items.Select(ToDomain).ToList(), p.TotalCount, p.PageNumber, p.PageSize);

        public static IReadOnlyList<Item> ToDomain(this ItemsByRarityResponse resp) =>
            resp.Items.Select(ToDomain).ToList();
    }
}
