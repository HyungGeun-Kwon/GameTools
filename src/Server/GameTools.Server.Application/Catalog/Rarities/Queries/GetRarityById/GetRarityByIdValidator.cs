using FluentValidation;

namespace GameTools.Server.Application.Catalog.Rarities.Queries.GetRarityById
{
    public class GetRarityByIdValidator : AbstractValidator<GetRarityByIdQuery>
    {
        public GetRarityByIdValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }
}
