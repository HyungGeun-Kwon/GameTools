using GameTools.Server.Application.Features.Items.Commands.Common.Specs;
using MediatR;

namespace GameTools.Server.Application.Features.Items.Commands.CreateItem
{
    public sealed record CreateItemCommand(CreateItemSpec Spec) : IRequest<CreateItemResult>;
}
