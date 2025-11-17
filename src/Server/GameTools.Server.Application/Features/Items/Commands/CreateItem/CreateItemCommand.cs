using GameTools.Server.Application.Features.Items.Models;
using MediatR;

namespace GameTools.Server.Application.Features.Items.Commands.CreateItem
{
    public sealed record CreateItemCommand(CreateItemPayload Payload) : IRequest<ItemReadModel>;
}
