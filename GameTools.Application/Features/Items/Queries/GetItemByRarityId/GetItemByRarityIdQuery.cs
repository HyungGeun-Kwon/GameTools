using GameTools.Application.Features.Items.Dtos;
using MediatR;

namespace GameTools.Application.Features.Items.Queries.GetItemById
{
    public sealed record GetItemByRarityIdQuery(byte RarityId) : IRequest<IReadOnlyList<ItemDto>>;
}
