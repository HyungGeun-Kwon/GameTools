using FluentValidation;
using GameTools.Domain.Common.Rules;

namespace GameTools.Application.Features.Items.Commands.UpdateItemsTvp
{
    public sealed class UpdateItemsTvpValidator : AbstractValidator<UpdateItemsTvpCommand>
    {
        public UpdateItemsTvpValidator()
        {
            RuleFor(x => x.Rows).NotEmpty();
            RuleForEach(x => x.Rows).ChildRules(row =>
            {
                row.RuleFor(r => r.Id).GreaterThan(0);
                row.RuleFor(r => r.Name).NotEmpty().MaximumLength(ItemRules.NameMax);
                row.RuleFor(r => r.Price).GreaterThanOrEqualTo(ItemRules.PriceMin);
                row.RuleFor(r => r.Description).MaximumLength(ItemRules.DescriptionMax);
                row.RuleFor(r => r.RarityId).GreaterThan((byte)0);
                row.RuleFor(r => r.RowVersionBase64).NotEmpty();
            });
            RuleFor(x => x.Actor).NotEmpty().MaximumLength(64);
        }
    }
}
