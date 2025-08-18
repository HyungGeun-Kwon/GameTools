using GameTools.Application.Abstractions.WriteStore;

namespace GameTools.Application.Features.Items.Commands.UpdateItemsTvp
{
    public sealed record UpdatedItemsTvpResult(int Id, UpdateStatusCode StatusCode, string? NewRowVersionBase64);
}
