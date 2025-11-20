using GameTools.Server.Application.Features.Items.Commands.Common.Bulk;

namespace GameTools.Server.Application.Features.Items.Commands.BulkDeleteItems
{
    public sealed record BulkDeleteItemsResult(IReadOnlyList<BulkResultRow> BulkResultRows);
}
