namespace GameTools.Contracts.Items.GetItemPage
{
    public sealed record ItemsPageRequest(int PageNumber = 1, int PageSize = 20, string? NameSearch = null, byte? RarityId = null);
}
