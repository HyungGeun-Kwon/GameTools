using GameTools.Application.Features.Items.Dtos;
using MediatR;

namespace GameTools.Application.Features.Items.Commands.UpdateItem
{
    public sealed record UpdateItemCommand(
        int Id,
        string Name,
        int Price,
        string? Description,
        byte RarityId
    ) : IRequest<ItemDto>;
}
