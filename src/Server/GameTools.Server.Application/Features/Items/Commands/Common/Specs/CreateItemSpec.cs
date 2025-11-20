namespace GameTools.Server.Application.Features.Items.Commands.Common.Specs
{
    public sealed record CreateItemSpec(string Name, int Price, Guid RarityId, string? Description = null);
}
