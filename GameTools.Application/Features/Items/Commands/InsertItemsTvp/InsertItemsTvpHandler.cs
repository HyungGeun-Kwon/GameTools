using GameTools.Application.Abstractions.Users;
using GameTools.Application.Abstractions.WriteStore;
using MediatR;

namespace GameTools.Application.Features.Items.Commands.InsertItemsTvp
{
    public sealed class InsertItemsTvpHandler(IItemWriteStore writeStore)
        : IRequestHandler<InsertItemsTvpCommand, IReadOnlyList<InsertedItemsTvpResult>>
    {
        public async Task<IReadOnlyList<InsertedItemsTvpResult>> Handle(InsertItemsTvpCommand request, CancellationToken ct)
        {
            IReadOnlyList<(int Id, byte[] NewRowVersion)> insertedList = await writeStore.InsertManyTvpAsync(request.ItemCreateDtos, ct);

            return insertedList
                .Select(insertedItem 
                    => new InsertedItemsTvpResult(
                        insertedItem.Id,
                        Convert.ToBase64String(insertedItem.NewRowVersion)))
                .ToList();
        }
    }
}
