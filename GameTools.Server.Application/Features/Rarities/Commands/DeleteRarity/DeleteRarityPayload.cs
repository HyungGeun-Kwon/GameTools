using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameTools.Server.Application.Features.Rarities.Commands.DeleteRarity
{
    public sealed record DeleteRarityPayload(byte Id, byte[] RowVersion);
}
