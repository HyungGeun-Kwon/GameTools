using FluentValidation;

namespace GameTools.Server.Application.Features.Rarities.Queries.GetRarityById
{
    public class GetRarityByIdValidator : AbstractValidator<GetRarityByIdQuery>
    {
        public GetRarityByIdValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }
}
