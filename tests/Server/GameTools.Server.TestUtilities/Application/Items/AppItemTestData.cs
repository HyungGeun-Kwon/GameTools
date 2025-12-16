using GameTools.Server.Application.Catalog.Items.Commands.Common.Specs;
using GameTools.Server.Application.Catalog.Items.Models;
using static GameTools.Server.TestUtilities.Domain.Items.DomainItemTestData;

namespace GameTools.Server.TestUtilities.Application.Items
{
    public static class AppItemTestData
    {
        public static byte[] ValidItemRowVersion()
            => Convert.FromBase64String("AAAAAAAAAAA=");

        public static ItemReadModel BuildDefaultItemReadModel(
            Guid? id = null,
            string? name = null,
            int? price = null,
            Guid? rarityId = null,
            string? description = null,
            byte[]? rowVersion = null)
            => new (
                Id: id ?? Guid.NewGuid(),
                Name: name ?? ValidItemNameValue(),
                Price: price ?? ValidItemPriceValue(),
                RarityId: rarityId ?? Guid.NewGuid(),
                Description: description ?? ValidItemDescriptionValue(),
                RowVersion: rowVersion ?? ValidItemRowVersion());

        public static CreateItemSpec BuildDefaultCreateItemSpec(
            string? name = null,
            int? price = null,
            string? description = null,
            Guid? rarityId = null)
            => new(
                Name: name ?? ValidItemNameValue(),
                Price: price ?? ValidItemPriceValue(),
                Description: description ?? ValidItemDescriptionValue(),
                RarityId: rarityId ?? Guid.NewGuid());

        public static UpdateItemSpec BuildDefaultUpdateItemSpec(
            Guid? id = null,
            string? name = null,
            int? price = null,
            string? description = null,
            Guid? rarityId = null,
            byte[]? rowVersion = null)
            => new(
                Id: id ?? Guid.NewGuid(),
                Name: name ?? ValidItemNameValue(),
                Price: price ?? ValidItemPriceValue(),
                Description: description ?? ValidItemDescriptionValue(),
                RarityId: rarityId ?? Guid.NewGuid(),
                RowVersion: rowVersion ?? ValidItemRowVersion());

        public static DeleteItemSpec BuildDefaultDeleteItemSpec(
            Guid? id = null,
            byte[]? rowVersion = null)
            => new(
                Id: id ?? Guid.NewGuid(),
                RowVersion: rowVersion ?? ValidItemRowVersion());
    }
}
