using FluentValidation;

namespace GameTools.Application.Features.Rarities.Commands.DeleteRarity
{
    public sealed class DeleteRarityValidator : AbstractValidator<DeleteRarityCommand>
    {
        public DeleteRarityValidator()
        {
            RuleFor(x => x.Id).GreaterThan((byte)0);
        }
    }
}
