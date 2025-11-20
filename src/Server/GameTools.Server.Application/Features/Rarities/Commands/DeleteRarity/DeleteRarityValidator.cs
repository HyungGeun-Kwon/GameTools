using FluentValidation;
using GameTools.Server.Application.Features.Rarities.Commands.Common.Specs;

namespace GameTools.Server.Application.Features.Rarities.Commands.DeleteRarity
{
    public sealed class DeleteRarityValidator : AbstractValidator<DeleteRarityCommand>
    {
        public DeleteRarityValidator(IValidator<DeleteRaritySpec> specValidator)
        {
            RuleFor(x => x.Spec).NotNull().SetValidator(specValidator);
        }
    }
}
