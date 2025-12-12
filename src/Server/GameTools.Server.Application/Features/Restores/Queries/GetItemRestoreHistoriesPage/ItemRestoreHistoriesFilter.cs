namespace GameTools.Server.Application.Features.Restores.Queries.GetItemRestoreHistoriesPage
{
    public sealed record ItemRestoreHistoriesFilter(
        IReadOnlyList<string>? CurrentUser,
        DateTime? FromUtc,
        DateTime? ToUtc,
        bool? DryOnly
    );
}
