namespace GameTools.Application.Features.Items.Queries.GetItemPage
{
    public sealed record ItemFilter(string? Search = null, byte? RarityId = null);
}
