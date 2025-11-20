using FluentValidation;
using GameTools.Server.Application.Features.Items.Commands.Common.Specs;

namespace GameTools.Server.Application.Features.Items.Commands.BulkUpdateItems
{
    public sealed class BulkUpdateItemsValidator : AbstractValidator<BulkUpdateItemsCommand>
    {
        public BulkUpdateItemsValidator(IValidator<UpdateItemSpec> specValidator)
        {
            RuleFor(x => x.Specs).NotNull();
            RuleForEach(x => x.Specs).SetValidator(specValidator);
        }
    }
}
