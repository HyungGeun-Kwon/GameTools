using MediatR;

namespace GameTools.Server.Application.Operations.Restores.Commands.RestoreItems
{
    public sealed record RestoreItemsCommand(RestoreItemsSpec Spec) : IRequest<RestoreItemsResult>;
}
