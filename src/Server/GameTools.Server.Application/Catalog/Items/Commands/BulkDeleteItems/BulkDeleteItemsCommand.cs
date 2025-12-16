using GameTools.Server.Application.Catalog.Items.Commands.Common.Specs;
using MediatR;

namespace GameTools.Server.Application.Catalog.Items.Commands.BulkDeleteItems
{
    public sealed record BulkDeleteItemsCommand(IReadOnlyList<DeleteItemSpec> Specs)
        : IRequest<BulkDeleteItemsResult>;
}
