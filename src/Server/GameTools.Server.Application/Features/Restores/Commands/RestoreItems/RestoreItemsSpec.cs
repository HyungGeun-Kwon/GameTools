namespace GameTools.Server.Application.Features.Restores.Commands.RestoreItems
{
    public sealed record RestoreItemsSpec(
        DateTime AsOfUtc, 
        IReadOnlyCollection<Guid>? ItemIds, // null = 전체복구, 0 = 허용하지 않음.
        string? Notes, 
        bool DryRun = false);
}
