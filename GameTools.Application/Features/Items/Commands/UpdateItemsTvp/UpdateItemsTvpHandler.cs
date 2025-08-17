using GameTools.Application.Abstractions.Users;
using GameTools.Application.Abstractions.WriteStore;
using GameTools.Application.Features.Items.Commands.Common;
using MediatR;

namespace GameTools.Application.Features.Items.Commands.UpdateItemsTvp
{
    public sealed class UpdateItemsTvpHandler(IItemWriteStore writeStore)
        : IRequestHandler<UpdateItemsTvpCommand, IReadOnlyList<UpdateItemResult>>
    {
        public async Task<IReadOnlyList<UpdateItemResult>> Handle(UpdateItemsTvpCommand request, CancellationToken ct)
        {
            var rows = request.Rows.Select(r => new ItemUpdateRow(
                r.Id, r.Name, r.Price, r.Description, r.RarityId, Convert.FromBase64String(r.RowVersionBase64)));

            var results = await writeStore.UpdateManyTvpAsync(rows, ct);

            return results
                .Select(r => new UpdateItemResult(
                    r.Id,
                    r.StatusCode,
                    r.NewRowVersion is null ? null : Convert.ToBase64String(r.NewRowVersion)))
                .ToList();
        }
    }
}
