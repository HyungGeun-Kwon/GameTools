using GameTools.Server.Domain.Features.Items.ValueObjects;
using GameTools.Server.Domain.Features.Rarities.ValueObjects;

namespace GameTools.Server.Domain.Tests.TestDatas.Rarities
{
    public static class RarityDomainTestData
    {
        public static string ValidGrade(char c = 'a')
            => new (c, RarityGrade.MinLength);

        public static string ValidColorCode(string code = "#AAAAAA")
            => code;
    }
}
