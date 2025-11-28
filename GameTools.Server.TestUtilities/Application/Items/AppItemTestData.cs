using GameTools.Server.Application.Features.Items.Commands.Common.Specs;
using GameTools.Server.Application.Features.Items.Commands.CreateItem;
using GameTools.Server.Application.Features.Items.Models;
using static GameTools.Server.TestUtilities.Domain.Items.DomainItemTestData;

namespace GameTools.Server.TestUtilities.Application.Items
{
    public static class AppItemTestData
    {
        public static ItemReadModel BuildValidItemReadModel(Guid? id = null)
            => new(
                Id: id ?? Guid.NewGuid(),
                Name: ValidItemNameValue(),
                Price: ValidItemPriceValue(),
                RarityId: Guid.NewGuid(),
                Description: ValidItemDescriptionValue(),
                RowVersion: [1, 2, 3, 4]);

        public static CreateItemSpec BuildValidCreateSpec(
            string? name = null,
            int? price = null,
            string? description = null,
            Guid? rarityId = null)
            => new(
                Name: name ?? ValidItemNameValue(),
                Price: price ?? ValidItemPriceValue(),
                Description: description ?? ValidItemDescriptionValue(),
                RarityId: rarityId ?? Guid.NewGuid());

        public static CreateItemCommand BuildValidCreateCommand(CreateItemSpec? spec = null)
            => new(spec ?? BuildValidCreateSpec());
    }
}
