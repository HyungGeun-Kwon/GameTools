using FluentValidation;
using GameTools.Server.Application.Catalog.Rarities.Commands.Common.Specs;

namespace GameTools.Server.Application.Catalog.Rarities.Commands.UpdateRarity
{
    public sealed class UpdateRarityValidator : AbstractValidator<UpdateRarityCommand>
    {
        public UpdateRarityValidator(IValidator<UpdateRaritySpec> specValidator)
        {
            RuleFor(x => x.Spec).NotNull().SetValidator(specValidator);
        }
    }
}
