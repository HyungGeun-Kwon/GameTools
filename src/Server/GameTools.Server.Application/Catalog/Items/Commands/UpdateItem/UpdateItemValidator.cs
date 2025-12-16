using FluentValidation;
using GameTools.Server.Application.Catalog.Items.Commands.Common.Specs;

namespace GameTools.Server.Application.Catalog.Items.Commands.UpdateItem
{
    public sealed class UpdateItemValidator : AbstractValidator<UpdateItemCommand>
    {
        public UpdateItemValidator(IValidator<UpdateItemSpec> specValidator)
        {
            RuleFor(x => x.Spec).NotNull().SetValidator(specValidator);
        }
    }
}
