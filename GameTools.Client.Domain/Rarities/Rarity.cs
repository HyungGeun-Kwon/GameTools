using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameTools.Client.Domain.Rarities
{
    public sealed record Rarity(byte Id, string Grade, string ColorCode, string RowVersionBase64);
}
