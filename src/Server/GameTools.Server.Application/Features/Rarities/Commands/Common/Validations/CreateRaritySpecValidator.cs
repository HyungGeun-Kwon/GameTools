using FluentValidation;
using GameTools.Server.Application.Features.Rarities.Commands.Common.Specs;
using GameTools.Server.Domain.Features.Rarities.ValueObjects;

namespace GameTools.Server.Application.Features.Rarities.Commands.Common.Validations
{
    public class CreateRaritySpecValidator : AbstractValidator<CreateRaritySpec>
    {
        public CreateRaritySpecValidator()
        {
            RuleFor(x => x.Grade)
                .NotEmpty()
                .MaximumLength(RarityGrade.MaxLength);
            RuleFor(x => x.NormalizedColorCode)
                .NotEmpty()
                .Length(RarityColorCode.Length)
                .Matches(RarityColorCode.HexColorRegex());
        }
    }
}
