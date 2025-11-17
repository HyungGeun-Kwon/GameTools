using GameTools.Server.Application.Common.Results;
using MediatR;

namespace GameTools.Server.Application.Features.Items.Commands.DeleteItem
{
    public sealed record DeleteItemCommand(DeleteItemPayload Payload) : IRequest<WriteStatusCode>;
}
