using FluentValidation;
using GameTools.Server.Application.Catalog.Rarities.Commands.Common.Specs;

namespace GameTools.Server.Application.Catalog.Rarities.Commands.Common.Validations
{
    public sealed class DeleteRaritySpecValidator : AbstractValidator<DeleteRaritySpec>
    {
        public DeleteRaritySpecValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.RowVersion).NotEmpty();
        }
    }
}
