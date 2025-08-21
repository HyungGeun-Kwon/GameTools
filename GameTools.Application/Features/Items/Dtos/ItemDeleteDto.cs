using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameTools.Application.Features.Items.Dtos
{
    public record ItemDeleteDto(int Id, string RowVersionBase64);
}
