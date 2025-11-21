using FluentValidation;
using GameTools.Server.Application.Features.Rarities.Commands.Common.Specs;
using GameTools.Server.Domain.Features.Rarities.ValueObjects;

namespace GameTools.Server.Application.Features.Rarities.Commands.Common.Validations
{
    public sealed class UpdateRaritySpecValidator : AbstractValidator<UpdateRaritySpec>
    {
        public UpdateRaritySpecValidator()
        {
            RuleFor(x => x.Id).NotEmpty();

            RuleFor(x => x.RowVersion).NotEmpty();

            RuleFor(x => x.Grade)
                .NotEmpty()
                .Must(x => !string.IsNullOrWhiteSpace(x))
                .MinimumLength(RarityGrade.MinLength)
                .MaximumLength(RarityGrade.MaxLength);

            RuleFor(x => x.NormalizedColorCode)
                .NotEmpty()
                .Length(RarityColorCode.Length)
                .Matches(RarityColorCode.HexColorRegex());
        }
    }
}
