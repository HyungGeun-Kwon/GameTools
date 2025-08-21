using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameTools.Application.Features.Rarities.Dtos
{
    public record RarityDeleteDto(byte Id, string RowVersionBase64);
}
