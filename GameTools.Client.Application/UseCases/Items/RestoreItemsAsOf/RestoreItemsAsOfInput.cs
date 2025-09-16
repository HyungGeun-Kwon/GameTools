namespace GameTools.Client.Application.UseCases.Items.RestoreItemsAsOf
{
    public sealed record RestoreItemsAsOfInput(DateTime AsOfUtc, int? ItemId, bool DryRun, string? Notes);
}
