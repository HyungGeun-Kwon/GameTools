using MediatR;

namespace GameTools.Server.Application.Features.Items.Commands.RestoreItemsAsOf
{
    public sealed record RestoreItemsAsOfCommand(RestoreItemsAsOfPayload Payload) : IRequest<RestoreItemsAsOfResult>;
}
