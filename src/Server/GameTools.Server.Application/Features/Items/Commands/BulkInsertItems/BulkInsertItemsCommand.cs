using GameTools.Server.Application.Features.Items.Commands.Common.Specs;
using MediatR;

namespace GameTools.Server.Application.Features.Items.Commands.BulkInsertItems
{
    public sealed record BulkInsertItemsCommand(IReadOnlyList<CreateItemSpec> Specs)
        : IRequest<BulkInsertItemsResult>;
}
