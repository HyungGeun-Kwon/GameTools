using FluentAssertions;
using GameTools.Server.Application.Common.Results;
using GameTools.Server.Application.Features.Items.Commands.CreateItem;
using GameTools.Server.Application.Features.Items.Commands.UpdateItem;
using GameTools.Server.Application.Features.Items.Models;
using GameTools.Server.Domain.Auditing;
using GameTools.Server.Domain.Entities;
using GameTools.Server.Infrastructure.Persistence;
using GameTools.Server.Infrastructure.Persistence.Stores.WriteStore;
using GameTools.Server.Infrastructure.Persistence.Works;
using GameTools.Server.Test.Utils;
using Microsoft.EntityFrameworkCore;

namespace GameTools.Server.Test.DataBase.ItemTest
{
    public class UpdateItemTests
    {
        [Fact]
        public async Task UpdateItem_SuccessInMemory()
        {
            await using var db = TestDataBase.CreateTestDbContext();
            var createdItem = await CreateItem(db);
            await UpdateItemSuccess(db, createdItem);
        }

        [Fact]
        public async Task UpdateItem_SuccessSqlServer()
        {
            await using var serverDb = await SqlServerTestDb.CreateAsync();
            var db = serverDb.Db;
            var createdItem = await CreateItem(db);
            await UpdateItemSuccess(db, createdItem);
        }

        [Fact]
        public async Task UpdateItem_SuccessCreateAuditItem()
        {
            await using var serverDb = await SqlServerTestDb.CreateAsync();
            var db = serverDb.Db;
            var createdItem = await CreateItem(db);
            
            var utcBeforeUpdate = DateTime.UtcNow;
            await UpdateItemSuccess(db, createdItem);

            List<ItemAudit> itemaudits = await db.Set<ItemAudit>().Select(ia => ia).ToListAsync();

            itemaudits.Count.Should().Be(2);
            ItemAudit? updateAudit = itemaudits.Find(ia => ia.Action == AuditAction.Update);
            updateAudit.Should().NotBeNull();
            updateAudit.AuditId.Should().BeGreaterThan(0);
            updateAudit.ItemId.Should().Be(createdItem.Id);
            updateAudit.BeforeJson.Should().NotBeNullOrEmpty();
            updateAudit.AfterJson.Should().NotBeNullOrEmpty();
            updateAudit.ChangedBy.Should().Be(new TestCurrentUser().UserIdOrName);
            updateAudit.ChangedAtUtc.Should().BeOnOrAfter(utcBeforeUpdate);
        }

        [Fact]
        public async Task UpdateItem_FailsWhenRenamingToSameName()
        {
            await using var serverDb = await SqlServerTestDb.CreateAsync();
            var db = serverDb.Db;
            var rarity = serverDb.SeedRarity();

            var handler = CreateItemTests.CreateHandler(db);
            var item1 = await handler.Handle(new CreateItemCommand(new CreateItemPayload("Item1", 1, rarity.Id)), CancellationToken.None);
            var item2 = await handler.Handle(new CreateItemCommand(new CreateItemPayload("Item2", 1, rarity.Id)), CancellationToken.None);

            var updater = UpdateItemHandler(db);
            var act = async () => await updater.Handle(new UpdateItemCommand(
                new UpdateItemPayload(item2.Id, "Item1", item2.Price, item2.Description, item2.RarityId, item2.RowVersion)), CancellationToken.None);

            await act.Should().ThrowAsync<Exception>();
        }

        [Fact]
        public async Task UpdateItem_SuccessWhenRarityChange()
        {
            await using var serverDb = await SqlServerTestDb.CreateAsync();
            var db = serverDb.Db;

            var r1 = serverDb.SeedRarity(grade: "COMMON", "#AAAAAA");
            var r2 = serverDb.SeedRarity(grade: "RARE", "#BBBBBB");

            var created = await CreateItem(db, r1);

            var updater = UpdateItemHandler(db);
            var updated = await updater.Handle(new UpdateItemCommand(new UpdateItemPayload(
                created.Id, created.Name, created.Price, created.Description, r2.Id, created.RowVersion)), CancellationToken.None);

            updated.ItemReadModel.Should().NotBeNull();

            updated.ItemReadModel.RarityId.Should().Be(r2.Id);
            var saved = await CreateItemTests.GetSavedAsync(db, updated.ItemReadModel.Id);
            saved.RarityId.Should().Be(r2.Id);
        }

        [Fact]
        public async Task UpdateItem_FailsWhenRarityNotFound()
        {
            await using var db = TestDataBase.CreateTestDbContext();
            var created = await CreateItem(db);

            var updater = UpdateItemHandler(db);
            // 존재하지 않는 Rarity로 업데이트 시도
            var act = async () => await updater.Handle(new UpdateItemCommand(new UpdateItemPayload(
                created.Id, created.Name, created.Price, created.Description, 250, created.RowVersion)), CancellationToken.None);

            await act.Should().ThrowAsync<Exception>();
        }

        [Fact]
        public async Task UpdateItem_FailsRowVersionMismatch()
        {
            await using var serverDb = await SqlServerTestDb.CreateAsync();

            var db = serverDb.Db;
            var currentUser = new TestCurrentUser();
            var handler = CreateItemTests.CreateHandler(db);

            var rarity = serverDb.SeedRarity();

            // 아이템 생성
            var cmd = new CreateItemCommand(new CreateItemPayload("Item1", 100, rarity.Id, "First"));
            var create = await handler.Handle(cmd, CancellationToken.None);

            // 서로 다른 컨텍스트 두 개 준비
            await using var db1 = serverDb.NewDb();
            await using var db2 = serverDb.NewDb();

            // 각각의 컨텍스트에서 아이템 조회
            var item1 = await db1.Items.SingleAsync(i => i.Id == create.Id);
            var item2 = await db2.Items.SingleAsync(i => i.Id == create.Id);

            // 가격 수정
            item1.SetPrice(item1.Price + 100);
            item2.SetPrice(item1.Price + 200);

            // 첫번째 컨텍스트에서 업데이트
            var uow1 = new UnitOfWork(db1, currentUser);
            await uow1.SaveChangesAsync();

            // 두번째 컨텍스트에서 업데이트
            var uow2 = new UnitOfWork(db2, currentUser);
            var act = async () => await uow2.SaveChangesAsync();

            await act.Should().ThrowAsync<Exception>();
        }

        [Fact]
        public async Task UpdateItem_NoUpdateRowVersionAndAuditWhenSameItemUpdated()
        {
            await using var serverDb = await SqlServerTestDb.CreateAsync();
            var db = serverDb.Db;

            var created = await CreateItem(db);

            var updater = UpdateItemHandler(db);
            var updated = await updater.Handle(new UpdateItemCommand(new UpdateItemPayload(
                created.Id, created.Name, created.Price, created.Description, created.RarityId, created.RowVersion)), CancellationToken.None);

            // 변경사항이 없다면 RowVersion은 변경되지 않아야 함
            updated.WriteStatusCode.Should().Be(WriteStatusCode.Success);
            updated.ItemReadModel.Should().NotBeNull();
            updated.ItemReadModel.RowVersion.Should().Equal(created.RowVersion);
            // 변경사항이 없다면 Audit도 생성되지 않아야 함
            var audits = await db.Set<ItemAudit>().Where(a => a.ItemId == created.Id && a.Action == AuditAction.Update).ToListAsync();
            audits.Should().BeEmpty();
        }

        private static async Task UpdateItemSuccess(AppDbContext db, ItemReadModel createdItem)
        {
            var handler = UpdateItemHandler(db);
            var updatedItem = await handler.Handle(
                new UpdateItemCommand(new UpdateItemPayload(
                    createdItem.Id,
                    createdItem.Name,
                    createdItem.Price + 100,
                    createdItem.Description,
                    createdItem.RarityId,
                    createdItem.RowVersion)), CancellationToken.None);

            // 반환 DTO 기본값 검증
            updatedItem.Should().NotBeNull();
            updatedItem.ItemReadModel.Should().NotBeNull();
            updatedItem.ItemReadModel.Price.Should().Be(createdItem.Price + 100);

            // RowVersion변경 확인
            updatedItem.ItemReadModel.RowVersion.Should().NotBeEquivalentTo(createdItem.RowVersion);

            await AssertDtoMatchesEntityAsync(db, updatedItem.ItemReadModel);
        }

        private static async Task<ItemReadModel> CreateItem(AppDbContext db, Rarity? rarity = null)
        {
            var handler = CreateItemTests.CreateHandler(db);
            rarity = rarity is null ? TestDataBase.SeedRarity(db) : rarity;
            var cmd = new CreateItemCommand(new CreateItemPayload("Item1", 100, rarity.Id, "First Item"));
            return await handler.Handle(cmd, CancellationToken.None);
        }

        internal static UpdateItemHandler UpdateItemHandler(AppDbContext db)
            => new(
                new ItemWriteStore(db, new TestCurrentUser()),
                new RarityWriteStore(db),
                new UnitOfWork(db, new TestCurrentUser()));

        // DTO / 저장 엔티티 일치 검증 헬퍼
        private static async Task AssertDtoMatchesEntityAsync(AppDbContext db, ItemReadModel itemReadModel)
        {
            // 실제 저장된 엔티티 검증
            var saved = await CreateItemTests.GetSavedAsync(db, itemReadModel.Id);

            saved.Id.Should().Be(itemReadModel.Id);
            saved.Name.Should().Be(itemReadModel.Name);
            saved.Price.Should().Be(itemReadModel.Price);
            saved.Description.Should().Be(itemReadModel.Description);
            saved.RarityId.Should().Be(itemReadModel.RarityId);

            // RowVersion(Base64) 일치 검증
            saved.RowVersion.Should().BeEquivalentTo(itemReadModel.RowVersion);
        }
    }
}