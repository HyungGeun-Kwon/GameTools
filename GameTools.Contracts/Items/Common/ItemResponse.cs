using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameTools.Contracts.Items.Common
{
    public sealed record ItemResponse(
        int Id, string Name, int Price, string? Description,
        byte RarityId, string RarityGrade, string RarityColorCode,
        string RowVersionBase64
    );
}
