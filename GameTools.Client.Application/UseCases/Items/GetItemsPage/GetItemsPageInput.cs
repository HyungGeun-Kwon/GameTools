using GameTools.Client.Application.Common.Paging;

namespace GameTools.Client.Application.UseCases.Items.GetItemsPage
{
    public sealed record GetItemsPageInput(Pagination Pagination, ItemSearchFilter? Filter);
}
