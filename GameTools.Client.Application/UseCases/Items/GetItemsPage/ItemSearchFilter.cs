namespace GameTools.Client.Application.UseCases.Items.GetItemsPage
{
    public sealed record ItemSearchFilter(string? NameSearch = null, byte? RarityId = null);
}
