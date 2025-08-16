namespace GameTools.Application.Features.Items.Commands.Common
{
    public sealed record ItemUpdateRow(int Id, string Name, int Price, string? Description, byte RarityId, byte[] RowVersionOriginal);
}
