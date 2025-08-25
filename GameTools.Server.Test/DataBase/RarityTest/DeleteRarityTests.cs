using FluentAssertions;
using GameTools.Server.Application.Common.Results;
using GameTools.Server.Application.Features.Items.Commands.CreateItem;
using GameTools.Server.Application.Features.Items.Models;
using GameTools.Server.Application.Features.Rarities.Commands.CreateRarity;
using GameTools.Server.Application.Features.Rarities.Commands.DeleteRarity;
using GameTools.Server.Application.Features.Rarities.Models;
using GameTools.Server.Domain.Auditing;
using GameTools.Server.Domain.Entities;
using GameTools.Server.Infrastructure.Persistence;
using GameTools.Server.Infrastructure.Persistence.Stores.WriteStore;
using GameTools.Server.Infrastructure.Persistence.Works;
using GameTools.Server.Test.DataBase.ItemTest;
using GameTools.Server.Test.Utils;
using Microsoft.EntityFrameworkCore;

namespace GameTools.Server.Test.DataBase.RarityTest
{
    public class DeleteRarityTests
    {
        [Fact]
        public async Task DeleteRarity_SuccessInMemory()
        {
            await using var db = TestDataBase.CreateTestDbContext();
            var created = await CreateRarity(db, "Common", "#000000");

            var handler = DeleteHandler(db);
            var statusCode = await handler.Handle(new DeleteRarityCommand(new DeleteRarityPayload(created.Id, created.RowVersion)), CancellationToken.None);
            statusCode.Should().Be(WriteStatusCode.Success);

            // 삭제되었기 때문에 없어야함
            (await db.Set<Rarity>().AnyAsync(r => r.Id == created.Id)).Should().BeFalse();
        }

        [Fact]
        public async Task DeleteRarity_SuccessSqlServer()
        {
            await using var serverDb = await SqlServerTestDb.CreateAsync();
            var db = serverDb.Db;
            var created = await CreateRarity(db, "Common", "#000000");

            var handler = DeleteHandler(db);
            var statusCode = await handler.Handle(new DeleteRarityCommand(new DeleteRarityPayload(created.Id, created.RowVersion)), CancellationToken.None);
            statusCode.Should().Be(WriteStatusCode.Success);

            // 삭제되었기 때문에 없어야함
            (await db.Set<Item>().AnyAsync(i => i.Id == created.Id)).Should().BeFalse();
        }

        [Fact]
        public async Task DeleteRarity_Success_SqlServer_WithAudit()
        {
            await using var serverDb = await SqlServerTestDb.CreateAsync();
            var db = serverDb.Db;
            var created = await CreateRarity(db, "RARE", "#B2B2B2");

            var utcBeforeDelete = DateTime.UtcNow;

            var handler = DeleteHandler(db);
            var statusCode = await handler.Handle(new DeleteRarityCommand(new DeleteRarityPayload(created.Id, created.RowVersion)), CancellationToken.None);
            statusCode.Should().Be(WriteStatusCode.Success);

            // 감사: Insert + Delete = 2건
            var audits = await db.Set<RarityAudit>().Where(a => a.RarityId == created.Id).OrderBy(a => a.AuditId).ToListAsync();
            audits.Should().HaveCount(2);
            var deleteAudit = audits.Single(a => a.Action == AuditAction.Delete);
            deleteAudit.RarityId.Should().Be(created.Id);
            deleteAudit.BeforeJson.Should().NotBeNullOrEmpty();
            deleteAudit.AfterJson.Should().BeNullOrEmpty();
            deleteAudit.ChangedBy.Should().Be(new TestCurrentUser().UserIdOrName);
            deleteAudit.ChangedAtUtc.Should().BeOnOrAfter(utcBeforeDelete);
        }

        [Fact]
        public async Task DeleteRarity_FailsWhenRowVersionMismatch()
        {
            await using var serverDb = await SqlServerTestDb.CreateAsync();
            var db = serverDb.Db;
            var created = await CreateRarity(db, "Common", "#000000");

            // 서로 다른 컨텍스트 2개 준비
            await using var db1 = serverDb.NewDb();
            await using var db2 = serverDb.NewDb();

            // db1: Colorcode 수정.
            var rarity = await db1.Rarities.SingleAsync(r => r.Id == created.Id);
            rarity.SetColorCode("#FFFFFF");
            await new UnitOfWork(db1, new TestCurrentUser()).SaveChangesAsync();
                

            // db2: 예전 RowVersion으로 삭제 시도.

            var deleter = DeleteHandler(db2);
            var deleteCommand = new DeleteRarityCommand(new DeleteRarityPayload(created.Id, created.RowVersion));
            var statusCode = await deleter.Handle(deleteCommand, CancellationToken.None);
            statusCode.Should().Be(WriteStatusCode.VersionMismatch);
        }

        [Fact]
        public async Task DeleteItem_FailsWhenNotFound()
        {
            await using var db = TestDataBase.CreateTestDbContext();

            var deleteCommand = new DeleteRarityCommand(new DeleteRarityPayload(200, [1, 2, 3, 4]));
            var deleter = DeleteHandler(db);
            var statusCode = await deleter.Handle(deleteCommand, CancellationToken.None);
            statusCode.Should().Be(WriteStatusCode.NotFound);
        }

        [Fact]
        public async Task DeleteRarity_FailsWhenHasItems_Restricted_NoExtraAudit()
        {
            await using var serverDb = await SqlServerTestDb.CreateAsync();
            var db = serverDb.Db;

            var rarity = await CreateRarity(db, "Common", "#000000");
            // 이 Rarity를 참조하는 Item 생성
            await CreateItem(db, rarity.Id);

            // Adit 1건
            List<RarityAudit> auditsBefore = await db.Set<RarityAudit>().Select(r => r).ToListAsync();

            // 참조 중인 상태에서 삭제 시도 -> 예외 발생
            var handler = DeleteHandler(db);
            var act = async () => await handler.Handle(new DeleteRarityCommand(new DeleteRarityPayload(rarity.Id, rarity.RowVersion)), CancellationToken.None);
            await act.Should().ThrowAsync<Exception>();

            // 실패했으므로 Rarity는 남아있으며 Audit 추가되지 않음.
            (await db.Set<Rarity>().AnyAsync(r => r.Id == rarity.Id)).Should().BeTrue();

            var auditsAfter = await db.Set<RarityAudit>().Select(r => r).ToListAsync();

            auditsAfter.Count.Should().Be(auditsBefore.Count);
        }

        private static DeleteRarityHandler DeleteHandler(AppDbContext db)
            => new(
                new RarityWriteStore(db),
                new UnitOfWork(db, new TestCurrentUser())
            );


        private static async Task<RarityReadModel> CreateRarity(AppDbContext db, string grade, string color)
        {
            var create = new CreateRarityCommand(new CreateRarityPayload(grade, color));
            return await CreateRarityTests.CreateHandler(db).Handle(create, CancellationToken.None);
        }

        private static async Task<ItemReadModel> CreateItem(AppDbContext db, byte rarityId)
        {
            var handler = CreateItemTests.CreateHandler(db);
            var createItemPayload = new CreateItemPayload("RefItem", 100, rarityId, "ref");
            return await handler.Handle(new CreateItemCommand(createItemPayload), CancellationToken.None);
        }
    }
}
