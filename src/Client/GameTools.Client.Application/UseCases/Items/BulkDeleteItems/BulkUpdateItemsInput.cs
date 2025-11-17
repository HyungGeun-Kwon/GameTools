namespace GameTools.Client.Application.UseCases.Items.BulkDeleteItems
{
    public sealed record BulkDeleteItemsInput(IReadOnlyList<BulkDeleteItemInputRow> Inputs);
}
