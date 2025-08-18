using GameTools.Application.Features.Items.Dtos;
using MediatR;

namespace GameTools.Application.Features.Items.Commands.InsertItemsTvp
{
    public sealed record InsertItemsTvpCommand(IReadOnlyList<ItemCreateDto> ItemCreateDtos)
        : IRequest<IReadOnlyList<InsertedItemsTvpResult>>;
}
