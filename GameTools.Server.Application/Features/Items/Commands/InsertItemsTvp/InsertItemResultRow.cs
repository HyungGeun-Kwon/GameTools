using GameTools.Server.Application.Abstractions.Stores.WriteStore;

namespace GameTools.Server.Application.Features.Items.Commands.InsertItemsTvp
{
    public sealed record InsertItemResultRow(int? Id, BulkInsertStatusCode StatusCode, byte[]? RowVersion);
}
