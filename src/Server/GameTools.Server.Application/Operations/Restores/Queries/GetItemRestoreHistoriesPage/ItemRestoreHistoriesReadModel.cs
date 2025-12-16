namespace GameTools.Server.Application.Operations.Restores.Queries.GetItemRestoreHistoriesPage
{
    public sealed record ItemRestoreHistoriesReadModel(
        Guid Id,
        DateTime AsOfUtc,
        string CurrentUser,
        bool DryRun,
        DateTime StartedAtUtc,
        DateTime? EndedAtUtc,
        string? AffectedCounts,
        string? Notes,
        string? FiltersJson
    );
}
