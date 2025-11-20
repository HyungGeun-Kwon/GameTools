using GameTools.Server.Application.Features.Items.Commands.Common.Bulk;

namespace GameTools.Server.Application.Features.Items.Commands.BulkUpdateItems
{
    public sealed record BulkUpdateItemsResult(IReadOnlyList<BulkResultRow> BulkResultRows);
}