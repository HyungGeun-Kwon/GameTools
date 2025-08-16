namespace GameTools.Application.Features.Items.Commands.InsertItemsTvp
{
    public sealed record InsertItemRowRequest(string Name, int Price, string? Description, byte RarityId);
}
