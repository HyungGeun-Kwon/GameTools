using GameTools.Server.Application.Features.Items.Commands.Common.Specs;
using MediatR;

namespace GameTools.Server.Application.Features.Items.Commands.BulkDeleteItems
{
    public sealed record BulkDeleteItemsCommand(IReadOnlyList<DeleteItemSpec> Specs)
        : IRequest<BulkDeleteItemsResult>;
}
