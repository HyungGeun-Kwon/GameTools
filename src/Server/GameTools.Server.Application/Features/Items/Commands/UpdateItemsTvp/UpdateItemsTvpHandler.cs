using GameTools.Server.Application.Abstractions.Stores.WriteStore;
using MediatR;

namespace GameTools.Server.Application.Features.Items.Commands.UpdateItemsTvp
{
    public sealed class UpdateItemsTvpHandler(IItemWriteStore writeStore)
        : IRequestHandler<UpdateItemsTvpCommand, IReadOnlyList<UpdateItemResultRow>>
    {
        public async Task<IReadOnlyList<UpdateItemResultRow>> Handle(UpdateItemsTvpCommand request, CancellationToken ct)
        {
            IReadOnlyList<(int Id, byte[]? NewRowVersion, BulkUpdateStatusCode StatusCode)> results = await writeStore.UpdateManyTvpAsync(request.Rows, ct);

            return results
                .Select(r => new UpdateItemResultRow(
                    r.Id,
                    r.StatusCode,
                    r.NewRowVersion is null ? null : r.NewRowVersion))
                .ToList();
        }
    }
}
