using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameTools.Server.Application.Features.Rarities.Commands.CreateRarity
{
    public sealed record CreateRarityPayload(string Grade, string ColorCode);
}
