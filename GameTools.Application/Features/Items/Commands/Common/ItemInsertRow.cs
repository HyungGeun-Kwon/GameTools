namespace GameTools.Application.Features.Items.Commands.Common
{
    public sealed record ItemInsertRow(string Name, int Price, string? Description, byte RarityId);
}
