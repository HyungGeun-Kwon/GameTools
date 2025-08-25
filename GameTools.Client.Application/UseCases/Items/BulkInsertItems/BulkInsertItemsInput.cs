namespace GameTools.Client.Application.UseCases.Items.BulkInsertItems
{
    public sealed record BulkInsertItemsInput(IReadOnlyList<BulkInsertItemInputRow> Inputs);
}
