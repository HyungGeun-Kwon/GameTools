using GameTools.Server.Application.Abstractions.Stores.WriteStore;

namespace GameTools.Server.Application.Features.Items.Commands.UpdateItemsTvp
{
    public sealed record UpdateItemResultRow(int Id, UpdateStatusCode StatusCode, byte[]? NewRowVersion);
}
