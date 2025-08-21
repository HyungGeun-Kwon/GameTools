using FluentValidation;

namespace GameTools.Application.Features.Rarities.Commands.DeleteRarity
{
    public sealed class DeleteRarityValidator : AbstractValidator<DeleteRarityCommand>
    {
        public DeleteRarityValidator()
        {
            RuleFor(x => x.RarityDeleteDto).NotNull();
            RuleFor(x => x.RarityDeleteDto.RowVersionBase64).NotEmpty();
            RuleFor(x => x.RarityDeleteDto.Id).GreaterThan((byte)0);
        }
    }
}
