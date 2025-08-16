using MediatR;

namespace GameTools.Application.Features.Rarities.Commands.DeleteRarity
{
    public sealed record DeleteRarityCommand(byte Id) : IRequest;
}
