using MediatR;

namespace GameTools.Server.Application.Features.Items.Commands.DeleteItem
{
    public sealed record DeleteItemCommand(DeleteItemPayload Payload) : IRequest;
}
