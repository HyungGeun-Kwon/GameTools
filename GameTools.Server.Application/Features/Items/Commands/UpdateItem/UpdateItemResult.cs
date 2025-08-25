using GameTools.Server.Application.Common.Results;
using GameTools.Server.Application.Features.Items.Models;

namespace GameTools.Server.Application.Features.Items.Commands.UpdateItem
{
    public sealed record UpdateItemResult(WriteStatusCode WriteStatusCode, ItemReadModel? ItemReadModel);
}
