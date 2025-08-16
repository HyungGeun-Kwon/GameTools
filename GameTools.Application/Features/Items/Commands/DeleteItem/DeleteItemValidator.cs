using FluentValidation;

namespace GameTools.Application.Features.Items.Commands.DeleteItem
{
    public sealed class DeleteItemValidator : AbstractValidator<DeleteItemCommand>
    {
        public DeleteItemValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0);
        }
    }
}
