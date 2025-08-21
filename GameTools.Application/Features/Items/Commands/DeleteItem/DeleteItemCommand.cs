using GameTools.Application.Features.Items.Dtos;
using MediatR;

namespace GameTools.Application.Features.Items.Commands.DeleteItem
{
    public sealed record DeleteItemCommand(ItemDeleteDto ItemDeleteDto) : IRequest;
}
