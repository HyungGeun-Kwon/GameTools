using GameTools.Application.Abstractions.WriteStore;
using GameTools.Application.Features.Items.Commands.Common;
using MediatR;

namespace GameTools.Application.Features.Items.Commands.InsertItemsTvp
{
    public sealed class InsertItemsTvpHandler(IItemWriteStore writeStore)
        : IRequestHandler<InsertItemsTvpCommand, IReadOnlyList<InsertedItemResult>>
    {
        public async Task<IReadOnlyList<InsertedItemResult>> Handle(InsertItemsTvpCommand request, CancellationToken ct)
        {
            var rows = request.Rows.Select(r => new ItemInsertRow(r.Name, r.Price, r.Description, r.RarityId));
            var inserted = await writeStore.InsertManyTvpAsync(rows, request.Actor, ct);

            return inserted
                .Select(tuple => new InsertedItemResult(tuple.Id, Convert.ToBase64String(tuple.NewRowVersion)))
                .ToList();
        }
    }
}
