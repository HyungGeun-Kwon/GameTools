namespace GameTools.Client.Application.UseCases.Items.CreateItem
{
    public sealed record CreateItemInput(
        string Name,
        int Price,
        byte RarityId,
        string? Description
    );
}
