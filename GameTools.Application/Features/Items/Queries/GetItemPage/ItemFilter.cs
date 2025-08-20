using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameTools.Application.Features.Items.Queries.GetItemPage
{
    public sealed record ItemFilter(string? Search = null, byte? RarityId = null);
}
