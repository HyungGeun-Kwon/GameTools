namespace GameTools.Server.Application.Features.Restores.Commands.RestoreItems
{
    public sealed record RestoreItemsSpec(DateTime AsOfUtc, int? ItemId, bool DryRun, string? Notes);
}
