using FluentValidation;
using GameTools.Server.Application.Catalog.Items.Commands.Common.Specs;

namespace GameTools.Server.Application.Catalog.Items.Commands.DeleteItem
{
    public sealed class DeleteItemValidator : AbstractValidator<DeleteItemCommand>
    {
        public DeleteItemValidator(IValidator<DeleteItemSpec> specValidator)
        {
            RuleFor(x => x.Spec).NotNull().SetValidator(specValidator);
        }
    }
}
