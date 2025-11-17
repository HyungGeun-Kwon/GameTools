namespace GameTools.Server.Application.Features.Restores.Queries.GetRestoreRunsPage
{
    public sealed record RestoreRunReadModel(
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
