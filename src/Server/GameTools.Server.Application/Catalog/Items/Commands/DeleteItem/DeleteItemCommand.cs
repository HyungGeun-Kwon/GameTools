using GameTools.Server.Application.Catalog.Items.Commands.Common.Specs;
using MediatR;

namespace GameTools.Server.Application.Catalog.Items.Commands.DeleteItem
{
    public sealed record DeleteItemCommand(DeleteItemSpec Spec) : IRequest<DeleteItemResult>;
}
