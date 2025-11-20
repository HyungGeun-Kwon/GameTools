using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using GameTools.Server.Application.Features.Items.Queries.GetItemsPage;

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
