using GameTools.Server.Application.Catalog.Items.Commands.Common.Specs;
using MediatR;

namespace GameTools.Server.Application.Catalog.Items.Commands.UpdateItem
{
    public sealed record UpdateItemCommand(
        UpdateItemSpec Spec)
        : IRequest<UpdateItemResult>;
}
