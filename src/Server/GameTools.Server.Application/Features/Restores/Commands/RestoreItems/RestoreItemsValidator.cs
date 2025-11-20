using FluentValidation;

namespace GameTools.Server.Application.Features.Restores.Commands.RestoreItems
{
    public sealed class RestoreItemsValidator : AbstractValidator<RestoreItemsCommand>
    {
        public RestoreItemsValidator(IValidator<RestoreItemsSpec> specValidator)
        {
            RuleFor(x => x.Spec)
                .NotNull()
                .SetValidator(specValidator);
        }
    }
}
