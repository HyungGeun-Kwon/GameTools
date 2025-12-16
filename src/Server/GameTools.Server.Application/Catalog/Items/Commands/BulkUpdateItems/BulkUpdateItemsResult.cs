using GameTools.Server.Application.Catalog.Items.Commands.Common.Bulk;

namespace GameTools.Server.Application.Catalog.Items.Commands.BulkUpdateItems
{
    public sealed record BulkUpdateItemsResult(IReadOnlyList<BulkResultRow> BulkResultRows);
}