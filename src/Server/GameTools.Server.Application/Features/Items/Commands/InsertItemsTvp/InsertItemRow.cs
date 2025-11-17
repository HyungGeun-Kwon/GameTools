namespace GameTools.Server.Application.Features.Items.Commands.InsertItemsTvp
{
    public sealed record InsertItemRow(string Name, int Price, byte RarityId, string? Description = null);
}
