namespace GameTools.Client.Application.UseCases.Items.BulkUpdateItems
{
    public sealed record BulkUpdateItemsOutput(IReadOnlyList<BulkUpdateItemOutputRow> Outputs);
}
