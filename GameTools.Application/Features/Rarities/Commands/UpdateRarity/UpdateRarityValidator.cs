using FluentValidation;
using GameTools.Domain.Common.Rules;

namespace GameTools.Application.Features.Rarities.Commands.UpdateRarity
{
    public sealed class UpdateRarityValidator : AbstractValidator<UpdateRarityCommand>
    {
        public UpdateRarityValidator()
        {
            RuleFor(x => x.Id).GreaterThan((byte)0);
            RuleFor(x => x.Grade).NotEmpty().MaximumLength(RarityRules.GradeMax);

            RuleFor(x => x.NormalizedColorCode)
                .NotEmpty()
                .Matches(RarityRules.ColorHexRegex);
        }
    }
}
