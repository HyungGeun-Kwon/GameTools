using GameTools.Server.Application.Catalog.Items.Commands.Common.Specs;
using MediatR;

namespace GameTools.Server.Application.Catalog.Items.Commands.BulkInsertItems
{
    public sealed record BulkInsertItemsCommand(IReadOnlyList<CreateItemSpec> Specs)
        : IRequest<BulkInsertItemsResult>;
}
