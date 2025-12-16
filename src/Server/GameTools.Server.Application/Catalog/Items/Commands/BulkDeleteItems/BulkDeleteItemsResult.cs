using GameTools.Server.Application.Catalog.Items.Commands.Common.Bulk;

namespace GameTools.Server.Application.Catalog.Items.Commands.BulkDeleteItems
{
    public sealed record BulkDeleteItemsResult(IReadOnlyList<BulkResultRow> BulkResultRows);
}
