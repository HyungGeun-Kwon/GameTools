using FluentValidation;
using GameTools.Server.Application.Features.Items.Commands.Common.Specs;

namespace GameTools.Server.Application.Features.Items.Commands.DeleteItem
{
    public sealed class DeleteItemValidator : AbstractValidator<DeleteItemCommand>
    {
        public DeleteItemValidator(IValidator<DeleteItemSpec> specValidator)
        {
            RuleFor(x => x.Spec).NotNull().SetValidator(specValidator);
        }
    }
}
