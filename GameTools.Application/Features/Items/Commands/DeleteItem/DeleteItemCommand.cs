using MediatR;

namespace GameTools.Application.Features.Items.Commands.DeleteItem
{
    public sealed record DeleteItemCommand(int Id) : IRequest;
}
