using MediatR;

namespace GameTools.Server.Application.Features.Items.Commands.InsertItemsTvp
{
    public sealed record InsertItemsTvpCommand(IReadOnlyList<InsertItemRow> Rows)
        : IRequest<IReadOnlyList<InsertItemResultRow>>;
}
