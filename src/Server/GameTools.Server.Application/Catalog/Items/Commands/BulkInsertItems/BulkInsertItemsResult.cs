using GameTools.Server.Application.Catalog.Items.Commands.Common.Bulk;

namespace GameTools.Server.Application.Catalog.Items.Commands.BulkInsertItems
{
    public sealed record BulkInsertItemsResult(IReadOnlyList<BulkResultRow> BulkResultRows);
}
