using FluentValidation;
using GameTools.Domain.Common.Rules;

namespace GameTools.Application.Features.Rarities.Commands.CreateRarity
{
    public class CreateRarityValidator : AbstractValidator<CreateRarityCommand>
    {
        public CreateRarityValidator()
        {
            RuleFor(x => x.RarityCreateDto).NotEmpty();
            RuleFor(x => x.RarityCreateDto.Grade)
                .NotEmpty()
                .MaximumLength(RarityRules.GradeMax);
            RuleFor(x => x.NormalizedColorCode)
                .NotEmpty()
                .Matches(RarityRules.ColorHexRegex);
        }
    }
}
