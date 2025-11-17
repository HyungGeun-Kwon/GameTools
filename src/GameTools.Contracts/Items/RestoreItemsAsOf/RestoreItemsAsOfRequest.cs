namespace GameTools.Contracts.Items.RestoreItemsAsOf
{
    public sealed record RestoreItemsAsOfRequest(DateTime AsOfUtc, int? ItemId, bool DryRun, string? Notes);
}
