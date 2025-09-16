namespace GameTools.Contracts.Restores.Common
{
    public sealed record RestoreRunResponse(
        Guid RestoreId,
        DateTime AsOfUtc,
        string Actor,
        bool DryRun,
        DateTime StartedAtUtc,
        DateTime? EndedAtUtc,
        string? AffectedCounts,
        string? Notes,
        string? FiltersJson);
}
