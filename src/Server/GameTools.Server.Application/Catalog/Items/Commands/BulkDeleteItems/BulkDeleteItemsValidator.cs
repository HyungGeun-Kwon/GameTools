using FluentValidation;
using GameTools.Server.Application.Catalog.Items.Commands.Common.Specs;

namespace GameTools.Server.Application.Catalog.Items.Commands.BulkDeleteItems
{
    public sealed class BulkDeleteItemsValidator : AbstractValidator<BulkDeleteItemsCommand>
    {
        public BulkDeleteItemsValidator(IValidator<DeleteItemSpec> specValidator)
        {
            RuleFor(x => x.Specs).NotNull();
            RuleForEach(x => x.Specs).SetValidator(specValidator);
        }
    }
}
