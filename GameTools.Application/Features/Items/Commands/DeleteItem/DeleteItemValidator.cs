using FluentValidation;

namespace GameTools.Application.Features.Items.Commands.DeleteItem
{
    public sealed class DeleteItemValidator : AbstractValidator<DeleteItemCommand>
    {
        public DeleteItemValidator()
        {
            RuleFor(x => x.ItemDeleteDto).NotNull();
            RuleFor(x => x.ItemDeleteDto.RowVersionBase64).NotEmpty();
            RuleFor(x => x.ItemDeleteDto.Id).GreaterThan(0);
        }
    }
}
