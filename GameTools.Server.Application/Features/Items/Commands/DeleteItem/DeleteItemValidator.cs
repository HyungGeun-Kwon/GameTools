using FluentValidation;

namespace GameTools.Server.Application.Features.Items.Commands.DeleteItem
{
    public sealed class DeleteItemValidator : AbstractValidator<DeleteItemCommand>
    {
        public DeleteItemValidator()
        {
            RuleFor(x => x.Payload).NotNull();
            RuleFor(x => x.Payload.RowVersion).NotEmpty();
            RuleFor(x => x.Payload.Id).GreaterThan(0);
        }
    }
}
