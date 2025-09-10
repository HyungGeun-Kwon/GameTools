using FluentValidation;

namespace GameTools.Server.Application.Features.Items.Commands.DeleteItemsTvp
{
    public sealed class DeleteItemsTvpValidator : AbstractValidator<DeleteItemsTvpCommand>
    {
        public DeleteItemsTvpValidator()
        {
            RuleFor(x => x.Rows).NotEmpty();
            RuleForEach(x => x.Rows).ChildRules(row =>
            {
                row.RuleFor(r => r.Id).GreaterThan(0);
                row.RuleFor(r => r.RowVersion).NotEmpty();
            });
        }
    }
}
