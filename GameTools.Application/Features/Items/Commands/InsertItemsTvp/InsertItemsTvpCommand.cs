using MediatR;

namespace GameTools.Application.Features.Items.Commands.InsertItemsTvp
{
    public sealed record InsertItemsTvpCommand(IReadOnlyList<InsertItemRowRequest> Rows)
        : IRequest<IReadOnlyList<InsertedItemResult>>;
}
