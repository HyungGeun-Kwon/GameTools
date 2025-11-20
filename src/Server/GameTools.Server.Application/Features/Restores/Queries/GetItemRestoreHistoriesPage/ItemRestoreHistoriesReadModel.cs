namespace GameTools.Server.Application.Features.Restores.Queries.GetItemRestoreHistoriesPage
{
    public sealed record ItemRestoreHistoriesReadModel(
        Guid Id,
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
