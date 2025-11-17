using FluentValidation;

namespace GameTools.Server.Application.Features.Rarities.Commands.DeleteRarity
{
    public sealed class DeleteRarityValidator : AbstractValidator<DeleteRarityCommand>
    {
        public DeleteRarityValidator()
        {
            RuleFor(x => x.Payload).NotNull();
            RuleFor(x => x.Payload.RowVersion).NotEmpty();
            RuleFor(x => x.Payload.Id).GreaterThan((byte)0);
        }
    }
}
