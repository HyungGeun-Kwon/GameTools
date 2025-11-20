using GameTools.Server.Application.Features.Items.Commands.Common.Specs;
using MediatR;

namespace GameTools.Server.Application.Features.Items.Commands.DeleteItem
{
    public sealed record DeleteItemCommand(DeleteItemSpec Spec) : IRequest<DeleteItemResult>;
}
