using GameTools.Server.Domain.Features.Items.ValueObjects;

namespace GameTools.Server.Domain.Tests.TestDatas.Items
{
    public static class ItemDomainTestData
    {
        public static string ValidName(char c = 'a')
            => new (c, ItemName.MinLength);

        public static int ValidPrice(int? v = null)
            => v ?? ItemPrice.MinValue;

        public static string ValidDescription(char c = 'd')
            => new (c, Math.Min(10, ItemDescription.MaxLength));
    }
}
