using FluentValidation;
using GameTools.Server.Application.Catalog.Rarities.Commands.Common.Specs;

namespace GameTools.Server.Application.Catalog.Rarities.Commands.DeleteRarity
{
    public sealed class DeleteRarityValidator : AbstractValidator<DeleteRarityCommand>
    {
        public DeleteRarityValidator(IValidator<DeleteRaritySpec> specValidator)
        {
            RuleFor(x => x.Spec).NotNull().SetValidator(specValidator);
        }
    }
}
