namespace GameTools.Client.Application.Models.Restores
{
    public sealed record Restore(
        Guid RestoreId,
        DateTime AsOfUtc,
        string Actor,
        bool DryRun,
        DateTime StartedAtUtc,
        DateTime? EndedAtUtc,
        string? AffectedCounts,
        string? Notes,
        string? FiltersJson
    );
}
