using GameTools.Server.Application.Abstractions.Stores.WriteStore;
using MediatR;

namespace GameTools.Server.Application.Features.Items.Commands.DeleteItemsTvp
{
    public sealed class DeleteItemsTvpHandler(IItemWriteStore writeStore)
        : IRequestHandler<DeleteItemsTvpCommand, IReadOnlyList<DeleteItemResultRow>>
    {
        public async Task<IReadOnlyList<DeleteItemResultRow>> Handle(DeleteItemsTvpCommand request, CancellationToken ct)
        {
            IReadOnlyList<(int? Id, BulkDeleteStatusCode StatusCode)> deleteedList = await writeStore.DeleteManyTvpAsync(request.Rows, ct);

            return deleteedList
                .Select(deleteedItem 
                    => new DeleteItemResultRow(
                        deleteedItem.Id,
                        deleteedItem.StatusCode))
                .ToList();
        }
    }
}
