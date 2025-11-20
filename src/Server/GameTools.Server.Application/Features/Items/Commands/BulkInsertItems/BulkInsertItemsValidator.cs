using FluentValidation;
using GameTools.Server.Application.Features.Items.Commands.Common.Specs;

namespace GameTools.Server.Application.Features.Items.Commands.BulkInsertItems
{
    public sealed class BulkInsertItemsValidator : AbstractValidator<BulkInsertItemsCommand>
    {
        public BulkInsertItemsValidator(IValidator<CreateItemSpec> specValidator)
        {
            RuleFor(x => x.Specs).NotNull();
            RuleForEach(x => x.Specs).SetValidator(specValidator);
        }
    }
}
