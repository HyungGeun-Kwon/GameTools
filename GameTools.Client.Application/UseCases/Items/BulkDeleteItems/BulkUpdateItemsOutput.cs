namespace GameTools.Client.Application.UseCases.Items.BulkDeleteItems
{
    public sealed record BulkDeleteItemsOutput(IReadOnlyList<BulkDeleteItemOutputRow> Outputs);
}
