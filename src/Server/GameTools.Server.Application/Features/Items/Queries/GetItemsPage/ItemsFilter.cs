namespace GameTools.Server.Application.Features.Items.Queries.GetItemsPage
{
    public sealed record ItemsFilter(
        string? Search = null,
        IReadOnlyList<Guid>? RarityIds = null
    );
}
