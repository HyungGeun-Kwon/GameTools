using GameTools.Server.Application.Catalog.Items.Commands.Common.Specs;
using MediatR;

namespace GameTools.Server.Application.Catalog.Items.Commands.CreateItem
{
    public sealed record CreateItemCommand(CreateItemSpec Spec) : IRequest<CreateItemResult>;
}
