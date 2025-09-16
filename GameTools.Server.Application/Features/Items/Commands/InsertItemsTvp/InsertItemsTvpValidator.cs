using FluentValidation;
using GameTools.Server.Domain.Common.Rules;

namespace GameTools.Server.Application.Features.Items.Commands.InsertItemsTvp
{
    public sealed class InsertItemsTvpValidator : AbstractValidator<InsertItemsTvpCommand>
    {
        public InsertItemsTvpValidator()
        {
            RuleFor(x => x.Rows).NotEmpty();
            RuleForEach(x => x.Rows).ChildRules(row =>
            {
                row.RuleFor(i => i.Name).NotEmpty().MaximumLength(ItemRules.NameMax);
                row.RuleFor(i => i.Price).GreaterThanOrEqualTo(ItemRules.PriceMin);
                row.RuleFor(i => i.Description).MaximumLength(ItemRules.DescriptionMax);
                row.RuleFor(i => i.RarityId).GreaterThan((byte)0);
            });
        }
    }
}
