namespace GameTools.Client.Application.UseCases.Items.BulkUpdateItems
{
    public sealed record BulkUpdateItemsInput(IReadOnlyList<BulkUpdateItemInputRow> Inputs);
}
