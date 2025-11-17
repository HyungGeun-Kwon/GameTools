using FluentValidation;
using GameTools.Server.Domain.Common.Rules;

namespace GameTools.Server.Application.Features.Rarities.Commands.UpdateRarity
{
    public sealed class UpdateRarityValidator : AbstractValidator<UpdateRarityCommand>
    {
        public UpdateRarityValidator()
        {
            RuleFor(x => x.Payload).NotEmpty();
            RuleFor(x => x.Payload.Id).GreaterThan((byte)0);
            RuleFor(x => x.Payload.Grade).NotEmpty().MaximumLength(RarityRules.GradeMax);

            RuleFor(x => x.NormalizedColorCode)
                .NotEmpty()
                .Matches(RarityRules.ColorHexRegex);
        }
    }
}
