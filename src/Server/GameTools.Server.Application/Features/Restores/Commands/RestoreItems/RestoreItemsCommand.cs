using MediatR;

namespace GameTools.Server.Application.Features.Restores.Commands.RestoreItems
{
    public sealed record RestoreItemsCommand(RestoreItemsSpec Spec) : IRequest<RestoreItemsResult>;
}
