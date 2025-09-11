namespace GameTools.Server.Application.Features.Items.Commands.RestoreItemsAsOf
{
    public sealed record RestoreItemsAsOfPayload(DateTime AsOfUtc, int? ItemId, bool DryRun, string? Notes);
}
