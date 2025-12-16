using GameTools.Server.Application.Catalog.Items.Commands.Common.Specs;
using MediatR;

namespace GameTools.Server.Application.Catalog.Items.Commands.BulkUpdateItems
{
    public sealed record BulkUpdateItemsCommand(IReadOnlyList<UpdateItemSpec> Specs) 
        : IRequest<BulkUpdateItemsResult>;
}
