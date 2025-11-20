using FluentValidation;
using GameTools.Server.Application.Features.Rarities.Commands.Common.Specs;

namespace GameTools.Server.Application.Features.Rarities.Commands.UpdateRarity
{
    public sealed class UpdateRarityValidator : AbstractValidator<UpdateRarityCommand>
    {
        public UpdateRarityValidator(IValidator<UpdateRaritySpec> specValidator)
        {
            RuleFor(x => x.Spec).NotNull().SetValidator(specValidator);
        }
    }
}
