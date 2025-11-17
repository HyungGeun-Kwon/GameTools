using FluentValidation;
using GameTools.Server.Domain.Common.Rules;

namespace GameTools.Server.Application.Features.Rarities.Commands.CreateRarity
{
    public class CreateRarityValidator : AbstractValidator<CreateRarityCommand>
    {
        public CreateRarityValidator()
        {
            RuleFor(x => x.Payload).NotEmpty();
            RuleFor(x => x.Payload.Grade)
                .NotEmpty()
                .MaximumLength(RarityRules.GradeMax);
            RuleFor(x => x.NormalizedColorCode)
                .NotEmpty()
                .Matches(RarityRules.ColorHexRegex);
        }
    }
}
