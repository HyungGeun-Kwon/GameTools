using GameTools.Application.Features.Items.Dtos;
using MediatR;

namespace GameTools.Application.Features.Items.Commands.UpdateItemsTvp
{
    public sealed record UpdateItemsTvpCommand(IReadOnlyList<ItemUpdateDto> ItemUpdateDtos) 
        : IRequest<IReadOnlyList<UpdatedItemsTvpResult>>;
}
