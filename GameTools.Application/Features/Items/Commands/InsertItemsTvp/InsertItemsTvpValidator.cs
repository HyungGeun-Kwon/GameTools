using FluentValidation;
using GameTools.Domain.Common.Rules;

namespace GameTools.Application.Features.Items.Commands.InsertItemsTvp
{
    public sealed class InsertItemsTvpValidator : AbstractValidator<InsertItemsTvpCommand>
    {
        public InsertItemsTvpValidator()
        {
            RuleFor(x => x.Rows).NotEmpty();
            RuleForEach(x => x.Rows).ChildRules(row =>
            {
                row.RuleFor(r => r.Name).NotEmpty().MaximumLength(ItemRules.NameMax);
                row.RuleFor(r => r.Price).GreaterThanOrEqualTo(ItemRules.PriceMin);
                row.RuleFor(r => r.Description).MaximumLength(ItemRules.DescriptionMax);
                row.RuleFor(r => r.RarityId).GreaterThan((byte)0);
            });
        }
    }
}
