namespace GameTools.Server.Application.Features.Restores.Queries.GetItemRestoreHistoriesPage
{
    public sealed record ItemRestoreHistoriesFilter(
        IReadOnlyList<string>? Actors,
        DateTime? FromUtc,
        DateTime? ToUtc,
        bool? DryOnly
    );
}
