using MediatR;

namespace GameTools.Server.Application.Features.Items.Commands.UpdateItemsTvp
{
    public sealed record UpdateItemsTvpCommand(IReadOnlyList<UpdateItemRow> Rows) 
        : IRequest<IReadOnlyList<UpdateItemResultRow>>;
}
