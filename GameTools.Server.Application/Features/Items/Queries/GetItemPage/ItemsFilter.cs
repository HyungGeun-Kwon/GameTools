namespace GameTools.Server.Application.Features.Items.Queries.GetItemPage
{
    public sealed record ItemsFilter(string? Search = null, byte? RarityId = null);
}
