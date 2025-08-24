using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameTools.Client.Application.UseCases.CreateItem
{
    public sealed record CreateItemInput(
        string Name,
        int Price,
        byte RarityId,
        string? Description
    );
}
