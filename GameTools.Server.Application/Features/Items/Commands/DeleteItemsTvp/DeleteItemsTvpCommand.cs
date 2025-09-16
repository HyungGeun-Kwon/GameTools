using MediatR;

namespace GameTools.Server.Application.Features.Items.Commands.DeleteItemsTvp
{
    public sealed record DeleteItemsTvpCommand(IReadOnlyList<DeleteItemRow> Rows)
        : IRequest<IReadOnlyList<DeleteItemResultRow>>;
}
