namespace GameTools.Server.Application.Features.Items.Commands.CreateItem
{
    public sealed record CreateItemPayload(string Name, int Price, byte RarityId, string? Description = null);
}
