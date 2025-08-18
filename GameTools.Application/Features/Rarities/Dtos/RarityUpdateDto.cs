using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameTools.Application.Features.Rarities.Dtos
{
    public sealed record RarityUpdateDto(byte Id, string Grade, string ColorCode, string RowVersionBase64);
}
