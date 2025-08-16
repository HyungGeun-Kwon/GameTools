using MediatR;

namespace GameTools.Application.Features.Items.Commands.UpdateItemsTvp
{
    public sealed record UpdateItemsTvpCommand(
        IReadOnlyList<UpdateItemRowRequest> Rows,
        string Actor // 감사용
    ) : IRequest<IReadOnlyList<UpdateItemResult>>;
}
