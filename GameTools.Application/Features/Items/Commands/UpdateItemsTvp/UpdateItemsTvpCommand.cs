using MediatR;

namespace GameTools.Application.Features.Items.Commands.UpdateItemsTvp
{
    public sealed record UpdateItemsTvpCommand(IReadOnlyList<UpdateItemRowRequest> Rows) 
        : IRequest<IReadOnlyList<UpdateItemResult>>;
}
