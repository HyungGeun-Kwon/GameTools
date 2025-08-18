using GameTools.Application.Features.Items.Dtos;
using MediatR;

namespace GameTools.Application.Features.Items.Commands.UpdateItem
{
    public sealed record UpdateItemCommand(ItemUpdateDto ItemUpdateDto) : IRequest<ItemDto>;
}
