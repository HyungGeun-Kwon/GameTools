using GameTools.Application.Features.Items.Commands.CreateItem;
using GameTools.Application.Features.Items.Commands.DeleteItem;
using GameTools.Application.Features.Items.Dtos;
using GameTools.Domain.Entities;
using GameTools.Infrastructure.Persistence;
using GameTools.Infrastructure.Persistence.Stores.WriteStore;
using GameTools.Infrastructure.Persistence.Works;
using GameTools.Test.Utils;

namespace GameTools.Test.DataBase.ItemTest
{
    public class DeleteItemTest
    {
        [Fact]
        public async Task DeleteItem_SuccessInMemory()
        {
            await using var db = TestDataBase.CreateTestDbContext();
            var created = await CreateItem(db);

            var handler = DeleteHandler(db);
            await handler.Handle(new DeleteItemCommand(created.Id, created.RowVersionBase64), default);

            (await db.Set<Item>().AnyAsync(i => i.Id == created.Id)).Should().BeFalse();
        }


        private static DeleteItemHandler DeleteHandler(AppDbContext db)
            => new(
                new ItemWriteStore(db, new TestCurrentUser()),
                new UnitOfWork(db, new TestCurrentUser())
            );

        private static async Task<ItemDto> CreateItem(AppDbContext db)
        {
            var rarity = TestDataBase.SeedRarity(db);
            var create = new CreateItemCommand(new ItemCreateDto("ToDelete", 10, rarity.Id, "temp"));
            return await CreateItemTests.CreateHandler(db).Handle(create, default);
        }
    }
}
