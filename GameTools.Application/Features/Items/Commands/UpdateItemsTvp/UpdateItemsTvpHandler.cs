using GameTools.Application.Abstractions.Stores.WriteStore;
using MediatR;

namespace GameTools.Application.Features.Items.Commands.UpdateItemsTvp
{
    public sealed class UpdateItemsTvpHandler(IItemWriteStore writeStore)
        : IRequestHandler<UpdateItemsTvpCommand, IReadOnlyList<UpdatedItemsTvpResult>>
    {
        public async Task<IReadOnlyList<UpdatedItemsTvpResult>> Handle(UpdateItemsTvpCommand request, CancellationToken ct)
        {
            IReadOnlyList<(int Id, byte[]? NewRowVersion, UpdateStatusCode StatusCode)> results = await writeStore.UpdateManyTvpAsync(request.ItemUpdateDtos, ct);

            return results
                .Select(r => new UpdatedItemsTvpResult(
                    r.Id,
                    r.StatusCode,
                    r.NewRowVersion is null ? null : Convert.ToBase64String(r.NewRowVersion)))
                .ToList();
        }
    }
}
