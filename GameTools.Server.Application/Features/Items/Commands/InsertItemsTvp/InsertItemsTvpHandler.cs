using GameTools.Server.Application.Abstractions.Stores.WriteStore;
using MediatR;

namespace GameTools.Server.Application.Features.Items.Commands.InsertItemsTvp
{
    public sealed class InsertItemsTvpHandler(IItemWriteStore writeStore)
        : IRequestHandler<InsertItemsTvpCommand, IReadOnlyList<InsertItemResultRow>>
    {
        public async Task<IReadOnlyList<InsertItemResultRow>> Handle(InsertItemsTvpCommand request, CancellationToken ct)
        {
            IReadOnlyList<(int? Id, byte[]? NewRowVersion, BulkInsertStatusCode StatusCode)> insertedList = await writeStore.InsertManyTvpAsync(request.Rows, ct);

            return insertedList
                .Select(insertedItem 
                    => new InsertItemResultRow(
                        insertedItem.Id,
                        insertedItem.StatusCode,
                        insertedItem.NewRowVersion))
                .ToList();
        }
    }
}
