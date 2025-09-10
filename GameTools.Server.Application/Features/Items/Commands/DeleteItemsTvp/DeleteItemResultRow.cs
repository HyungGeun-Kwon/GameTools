using GameTools.Server.Application.Abstractions.Stores.WriteStore;

namespace GameTools.Server.Application.Features.Items.Commands.DeleteItemsTvp
{
    public sealed record DeleteItemResultRow(int? Id, BulkDeleteStatusCode StatusCode);
}
