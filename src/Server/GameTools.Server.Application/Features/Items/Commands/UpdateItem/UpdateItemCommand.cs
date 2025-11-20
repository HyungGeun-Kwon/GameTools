using GameTools.Server.Application.Features.Items.Commands.Common.Specs;
using MediatR;

namespace GameTools.Server.Application.Features.Items.Commands.UpdateItem
{
    public sealed record UpdateItemCommand(
        UpdateItemSpec Spec)
        : IRequest<UpdateItemResult>;
}
