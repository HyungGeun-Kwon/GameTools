using GameTools.Server.Application.Features.Items.Commands.Common.Bulk;

namespace GameTools.Server.Application.Features.Items.Commands.BulkInsertItems
{
    public sealed record BulkInsertItemsResult(IReadOnlyList<BulkResultRow> BulkResultRows);
}
