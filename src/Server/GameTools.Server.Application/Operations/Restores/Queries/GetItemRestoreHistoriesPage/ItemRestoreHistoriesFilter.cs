namespace GameTools.Server.Application.Operations.Restores.Queries.GetItemRestoreHistoriesPage
{
    public sealed record ItemRestoreHistoriesFilter(
        IReadOnlyList<string>? Actors,
        DateTime? FromUtc,
        DateTime? ToUtc,
        bool? DryOnly
    );
}
