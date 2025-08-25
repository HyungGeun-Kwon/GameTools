using MediatR;

namespace GameTools.Server.Application.Features.Items.Commands.UpdateItem
{
    public sealed record UpdateItemCommand(UpdateItemPayload Payload) : IRequest<UpdateItemResult>;
}
