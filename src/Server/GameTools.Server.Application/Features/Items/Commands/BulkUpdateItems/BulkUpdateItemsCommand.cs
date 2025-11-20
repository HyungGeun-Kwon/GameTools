using GameTools.Server.Application.Features.Items.Commands.Common.Specs;
using MediatR;

namespace GameTools.Server.Application.Features.Items.Commands.BulkUpdateItems
{
    public sealed record BulkUpdateItemsCommand(IReadOnlyList<UpdateItemSpec> Specs) 
        : IRequest<BulkUpdateItemsResult>;
}
