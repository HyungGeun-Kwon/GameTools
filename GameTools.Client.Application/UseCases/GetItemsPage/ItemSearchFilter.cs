using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameTools.Client.Application.UseCases.GetItemsPage
{
    public sealed record ItemSearchFilter(string? NameSearch = null, byte? RarityId = null);
}
