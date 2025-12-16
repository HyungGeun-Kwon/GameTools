using GameTools.Server.Application.Catalog.Items.Models;
using MediatR;

namespace GameTools.Server.Application.Catalog.Items.Queries.GetItemById
{
    public sealed record GetItemByIdQuery(Guid Id) : IRequest<ItemReadModel>;
}
