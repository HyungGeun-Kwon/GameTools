using FluentAssertions;
using GameTools.Application.Features.Items.Commands.CreateItem;
using GameTools.Application.Features.Items.Commands.DeleteItem;
using GameTools.Application.Features.Items.Dtos;
using GameTools.Domain.Auditing;
using GameTools.Domain.Entities;
using GameTools.Infrastructure.Persistence;
using GameTools.Infrastructure.Persistence.Stores.WriteStore;
using GameTools.Infrastructure.Persistence.Works;
using GameTools.Test.Utils;
using Microsoft.EntityFrameworkCore;

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
            await handler.Handle(new DeleteItemCommand(new ItemDeleteDto(created.Id, created.RowVersionBase64)), CancellationToken.None);

            (await db.Set<Item>().AnyAsync(i => i.Id == created.Id)).Should().BeFalse();
        }

        [Fact]
        public async Task DeleteItem_SuccessSqlServer()
        {
            await using var serverDb = await SqlServerTestDb.CreateAsync();
            var db = serverDb.Db;
            var created = await CreateItem(db);

            var handler = DeleteHandler(db);
            await handler.Handle(new DeleteItemCommand(new ItemDeleteDto(created.Id, created.RowVersionBase64)), CancellationToken.None);

            (await db.Set<Item>().AnyAsync(i => i.Id == created.Id)).Should().BeFalse();
        }

        [Fact]
        public async Task DeleteItem_SuccessCreateAuditItem()
        {
            await using var serverDb = await SqlServerTestDb.CreateAsync();
            var db = serverDb.Db;
            var created = await CreateItem(db);

            var utcBeforeDelete = DateTime.UtcNow;
            var handler = DeleteHandler(db);
            await handler.Handle(new DeleteItemCommand(new ItemDeleteDto(created.Id, created.RowVersionBase64)), CancellationToken.None);

            (await db.Set<Item>().AnyAsync(i => i.Id == created.Id)).Should().BeFalse();

            var itemAudits = await db.Set<ItemAudit>().Select(ia => ia).ToListAsync();

            itemAudits.Count.Should().Be(2);
            var deleteAudit = itemAudits.Single(a => a.Action == AuditAction.Delete);
            deleteAudit.AuditId.Should().BeGreaterThan(0);
            deleteAudit.ItemId.Should().Be(created.Id);
            deleteAudit.BeforeJson.Should().NotBeNullOrEmpty();
            deleteAudit.AfterJson.Should().BeNullOrEmpty();
            deleteAudit.ChangedBy.Should().Be(new TestCurrentUser().UserIdOrName);
            deleteAudit.ChangedAtUtc.Should().BeOnOrAfter(utcBeforeDelete);
        }

        [Fact]
        public async Task DeleteItem_FailsWhenRowVersionMismatch()
        {
            await using var serverDb = await SqlServerTestDb.CreateAsync();
            var db = serverDb.Db;

            var created = await CreateItem(db);

            await using var db1 = serverDb.NewDb();
            await using var db2 = serverDb.NewDb();

            var currentUser = new TestCurrentUser();

            // db1: 가격 변경
            var item1 = await db1.Items.SingleAsync(i => i.Id == created.Id);
            item1.SetPrice(item1.Price + 1);
            await new UnitOfWork(db1, currentUser).SaveChangesAsync();

            // db2: 예전 RowVersion으로 삭제 시도
            var deleteCommand = new DeleteItemCommand(new ItemDeleteDto(created.Id, created.RowVersionBase64));
            var deleter = DeleteHandler(db2);
            var act = async () => await deleter.Handle(deleteCommand, CancellationToken.None);

            await act.Should().ThrowAsync<Exception>();
        }

        [Fact]
        public async Task DeleteItem_FailsWhenNotFound()
        {
            await using var db = TestDataBase.CreateTestDbContext();

            var handler = DeleteHandler(db);
            var act = async () => await handler.Handle(
                new DeleteItemCommand(new ItemDeleteDto(999, Convert.ToBase64String([1, 2, 3, 4]))), CancellationToken.None);

            await act.Should().ThrowAsync<Exception>();
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
            return await CreateItemTests.CreateHandler(db).Handle(create, CancellationToken.None);
        }
    }
}
