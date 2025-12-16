using FluentValidation;
using GameTools.Server.Application.Catalog.Rarities.Commands.Common.Specs;

namespace GameTools.Server.Application.Catalog.Rarities.Commands.CreateRarity
{
    public class CreateRarityValidator : AbstractValidator<CreateRarityCommand>
    {
        public CreateRarityValidator(IValidator<CreateRaritySpec> specValidator)
        {
            RuleFor(x => x.Spec).NotNull().SetValidator(specValidator);
        }
    }
}
