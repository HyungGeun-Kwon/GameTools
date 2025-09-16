namespace GameTools.Client.Application.UseCases.Items.BulkInsertItems
{
    public sealed record BulkInsertItemsOutput(IReadOnlyList<BulkInsertItemOutputRow> Outputs);
}