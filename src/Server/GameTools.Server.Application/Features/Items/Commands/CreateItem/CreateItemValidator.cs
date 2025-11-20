using FluentValidation;
using GameTools.Server.Application.Features.Items.Commands.Common.Specs;

namespace GameTools.Server.Application.Features.Items.Commands.CreateItem
{
    public sealed class CreateItemValidator : AbstractValidator<CreateItemCommand>
    {
        public CreateItemValidator(IValidator<CreateItemSpec> specValidator)
        {
            RuleFor(x => x.Spec).NotNull().SetValidator(specValidator);
        }
    }
}
