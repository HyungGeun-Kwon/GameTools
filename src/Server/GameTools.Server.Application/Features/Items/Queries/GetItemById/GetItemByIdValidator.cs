using FluentValidation;

namespace GameTools.Server.Application.Features.Items.Queries.GetItemById
{
    public sealed class GetItemByIdValidator : AbstractValidator<GetItemByIdQuery>
    {
        public GetItemByIdValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }
}
