using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameTools.Server.Application.Features.Rarities.Commands.UpdateRarity
{
    public sealed record UpdateRarityPayload(byte Id, string Grade, string ColorCode, byte[] RowVersion);
}
