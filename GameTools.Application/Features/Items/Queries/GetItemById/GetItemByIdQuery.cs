using GameTools.Application.Features.Items.Dtos;
using MediatR;

namespace GameTools.Application.Features.Items.Queries.GetItemById
{
    public sealed record GetItemByIdQuery(int Id) : IRequest<ItemDto?>;
}
