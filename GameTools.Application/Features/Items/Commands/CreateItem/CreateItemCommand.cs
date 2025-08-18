using GameTools.Application.Features.Items.Dtos;
using MediatR;

namespace GameTools.Application.Features.Items.Commands.CreateItem
{
    public sealed record CreateItemCommand(ItemCreateDto ItemCreateDto) : IRequest<ItemDto>;
}
