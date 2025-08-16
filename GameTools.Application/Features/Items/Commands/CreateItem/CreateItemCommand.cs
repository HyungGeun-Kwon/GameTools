using GameTools.Application.Features.Items.Dtos;
using MediatR;

namespace GameTools.Application.Features.Items.Commands.CreateItem
{
    public sealed record CreateItemCommand(
        string Name,
        int Price,
        string? Description,
        byte RarityId
    ) : IRequest<ItemDto>;
}
