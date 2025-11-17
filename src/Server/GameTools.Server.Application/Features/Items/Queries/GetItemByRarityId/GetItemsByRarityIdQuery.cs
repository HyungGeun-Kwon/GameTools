using GameTools.Server.Application.Features.Items.Models;
using MediatR;

namespace GameTools.Server.Application.Features.Items.Queries.GetItemById
{
    // TODO : 추후에 Page 기반으로 변경
    public sealed record GetItemsByRarityIdQuery(byte RarityId) : IRequest<IReadOnlyList<ItemReadModel>>;
}
