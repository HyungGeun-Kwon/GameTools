using GameTools.Server.Application.Features.Items.Models;
using MediatR;

namespace GameTools.Server.Application.Features.Items.Queries.GetItemById
{
    public sealed record GetItemByIdQuery(int Id) : IRequest<ItemReadModel?>;
}
