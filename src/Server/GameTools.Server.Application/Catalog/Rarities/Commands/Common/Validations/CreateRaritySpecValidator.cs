using FluentValidation;
using GameTools.Server.Application.Catalog.Rarities.Commands.Common.Specs;
using GameTools.Server.Domain.Catalog.Rarities.ValueObjects;

namespace GameTools.Server.Application.Catalog.Rarities.Commands.Common.Validations
{
    public class CreateRaritySpecValidator : AbstractValidator<CreateRaritySpec>
    {
        public CreateRaritySpecValidator()
        {
            RuleFor(x => x.Grade)
                .NotEmpty()
                .Must(x => !string.IsNullOrWhiteSpace(x))
                .MaximumLength(RarityGrade.MaxLength);
            RuleFor(x => x.NormalizedColorCode)
                .NotEmpty()
                .Length(RarityColorCode.Length)
                .Matches(RarityColorCode.HexColorRegex());
        }
    }
}
