using FluentValidation;
using GameTools.Server.Application.Catalog.Items.Commands.Common.Specs;

namespace GameTools.Server.Application.Catalog.Items.Commands.Common.Validations
{
    public sealed class DeleteItemSpecValidator : AbstractValidator<DeleteItemSpec>
    {
        public DeleteItemSpecValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.RowVersion).NotNull().NotEmpty();
        }
    }
}
