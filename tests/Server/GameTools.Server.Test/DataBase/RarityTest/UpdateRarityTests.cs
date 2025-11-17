using FluentAssertions;
using GameTools.Server.Application.Common.Results;
using GameTools.Server.Application.Features.Rarities.Commands.CreateRarity;
using GameTools.Server.Application.Features.Rarities.Commands.UpdateRarity;
using GameTools.Server.Application.Features.Rarities.Models;
using GameTools.Server.Domain.Auditing;
using GameTools.Server.Domain.Entities;
using GameTools.Server.Infrastructure.Persistence;
using GameTools.Server.Infrastructure.Persistence.Stores.WriteStore;
using GameTools.Server.Infrastructure.Persistence.Works;
using GameTools.Server.Test.Utils;
using Microsoft.EntityFrameworkCore;

namespace GameTools.Server.Test.DataBase.RarityTest
{
    public class UpdateRarityTests
    {
        [Fact]
        public async Task UpdateRarity_SuccessInMemory()
        {
            await using var db = TestDataBase.CreateTestDbContext();
            var created = await CreateRarity(db, "Common", "#000000");

            var handler = UpdateHandler(db);
            var updated = await handler.Handle(
                new UpdateRarityCommand(new UpdateRarityPayload(
                    created.Id, "Common_Fix", "#111111", created.RowVersion)), CancellationToken.None);

            updated.WriteStatusCode.Should().Be(WriteStatusCode.Success);
            updated.RarityReadModel.Should().NotBeNull();
            updated.RarityReadModel.Grade.Should().Be("Common_Fix");
            updated.RarityReadModel.ColorCode.Should().Be("#111111");
            updated.RarityReadModel.RowVersion.Should().NotEqual(created.RowVersion);

            await AssertDtoMatchesEntityAsync(db, updated.RarityReadModel);
        }

        [Fact]
        public async Task UpdateRarity_SuccessSqlServer()
        {
            await using var serverDb = await SqlServerTestDb.CreateAsync();
            var db = serverDb.Db;
            var created = await CreateRarity(db, "Common", "#000000");

            var handler = UpdateHandler(db);
            var updated = await handler.Handle(
                new UpdateRarityCommand(new UpdateRarityPayload(
                    created.Id, "Common_Fix", "#111111", created.RowVersion)), CancellationToken.None);

            updated.WriteStatusCode.Should().Be(WriteStatusCode.Success);
            updated.RarityReadModel.Should().NotBeNull();
            updated.RarityReadModel.Grade.Should().Be("Common_Fix");
            updated.RarityReadModel.ColorCode.Should().Be("#111111");
            updated.RarityReadModel.RowVersion.Should().NotEqual(created.RowVersion);

            await AssertDtoMatchesEntityAsync(db, updated.RarityReadModel);
        }

        [Fact]
        public async Task UpdateRarity_SuccessCreateAuditRarity()
        {
            await using var serverDb = await SqlServerTestDb.CreateAsync();
            var db = serverDb.Db;
            var created = await CreateRarity(db, "Common", "#000000");

            var utcBeforeUpdate = DateTime.UtcNow;

            var handler = UpdateHandler(db);
            var updated = await handler.Handle(
                new UpdateRarityCommand(new UpdateRarityPayload(
                    created.Id, "Common_Fix", "#111111", created.RowVersion)), CancellationToken.None);

            updated.WriteStatusCode.Should().Be(WriteStatusCode.Success);
            updated.RarityReadModel.Should().NotBeNull();
            updated.RarityReadModel.Id.Should().Be(created.Id);

            var audits = await db.Set<RarityAudit>().Select(r => r).ToListAsync();
            audits.Count.Should().Be(2);
            var updateAudit = audits.Single(a => a.Action == AuditAction.Update);
            updateAudit.AuditId.Should().BeGreaterThan(0);
            updateAudit.RarityId.Should().Be(created.Id);
            updateAudit.BeforeJson.Should().NotBeNullOrEmpty();
            updateAudit.AfterJson.Should().NotBeNullOrEmpty();
            updateAudit.ChangedBy.Should().Be(new TestCurrentUser().UserIdOrName);
            updateAudit.ChangedAtUtc.Should().BeOnOrAfter(utcBeforeUpdate);
        }

        [Fact]
        public async Task UpdateRarity_FailsWhenUpdateSameGrade()
        {
            await using var serverDb = await SqlServerTestDb.CreateAsync();
            var db = serverDb.Db;

            var rarity1 = await CreateRarity(db, "Grade_A", "#111111");
            var rarity2 = await CreateRarity(db, "Grade_B", "#222222");

            var handler = UpdateHandler(db);
            var act = async () => await handler.Handle(
                new UpdateRarityCommand(new UpdateRarityPayload(
                    rarity2.Id, "Grade_A", rarity2.ColorCode, rarity2.RowVersion)), CancellationToken.None);

            await act.Should().ThrowAsync<Exception>();
        }

        [Fact]
        public async Task UpdateRarity_FailsWhenUpdateSameColorCode()
        {
            await using var serverDb = await SqlServerTestDb.CreateAsync();
            var db = serverDb.Db;

            var rarity1 = await CreateRarity(db, "Grade_A", "#111111");
            var rarity2 = await CreateRarity(db, "Grade_B", "#222222");

            var handler = UpdateHandler(db);
            var act = async () => await handler.Handle(
                new UpdateRarityCommand(new UpdateRarityPayload(
                    rarity2.Id, rarity2.Grade, "#111111", rarity2.RowVersion)), CancellationToken.None);

            await act.Should().ThrowAsync<Exception>();
        }

        [Fact]
        public async Task UpdateRarity_FailsRowVersionMismatch()
        {
            await using var serverDb = await SqlServerTestDb.CreateAsync();

            var db = serverDb.Db;
            // Rarity 생성
            var created = await CreateRarity(db, "Common", "#000000");

            // 서로 다른 컨텍스트 두개 준비
            await using var db1 = serverDb.NewDb();
            await using var db2 = serverDb.NewDb();

            // 각 컨텍스트에서 Rarity를 조회
            var rarity1 = await db1.Set<Rarity>().SingleAsync(r => r.Id == created.Id);
            var rarity2 = await db2.Set<Rarity>().SingleAsync(r => r.Id == created.Id);

            // 수정
            rarity1.SetColorCode("#111111");
            rarity2.SetColorCode("#222222");

            // 첫번째 컨텍스트에서 업데이트
            var uow1 = new UnitOfWork(db1, new TestCurrentUser());
            await uow1.SaveChangesAsync();

            // 두번째 컨텍스트에서 업데이트
            var uow2 = new UnitOfWork(db2, new TestCurrentUser());
            var act = async () => await uow2.SaveChangesAsync();

            await act.Should().ThrowAsync<Exception>();
        }

        [Fact]
        public async Task UpdateRarity_NoUpdateRowVersionAndAuditWhenSameRarityUpdated()
        {
            await using var serverDb = await SqlServerTestDb.CreateAsync();
            var db = serverDb.Db;

            var created = await CreateRarity(db, "Common", "#000000");

            var handler = UpdateHandler(db);
            var updated = await handler.Handle(
                new UpdateRarityCommand(new UpdateRarityPayload(
                    created.Id, created.Grade, created.ColorCode, created.RowVersion)), CancellationToken.None);

            updated.WriteStatusCode.Should().Be(WriteStatusCode.Success);
            updated.RarityReadModel.Should().NotBeNull();
            // 변경사항이 없다면 RowVersion은 동일해야 함
            updated.RarityReadModel.RowVersion.Should().Equal(created.RowVersion);

            // 변경사항이 없다면 Audit(Update)도 없어야 함
            var audits = await db.Set<RarityAudit>()
                                 .Where(a => a.RarityId == created.Id && a.Action == AuditAction.Update)
                                 .ToListAsync();

            audits.Should().BeEmpty();
        }

        [Fact]
        public async Task UpdateRarity_FailsWhenNotFound()
        {
            await using var db = TestDataBase.CreateTestDbContext();
            var handler = UpdateHandler(db);

            var result = await handler.Handle(
                new UpdateRarityCommand(new UpdateRarityPayload(
                    250, "None", "#000000", [1, 2, 3, 4])), CancellationToken.None);
            result.WriteStatusCode.Should().Be(WriteStatusCode.NotFound);
            result.RarityReadModel.Should().BeNull();
        }

        private static async Task<RarityReadModel> CreateRarity(AppDbContext db, string grade, string color)
        {
            var handler = new CreateRarityHandler(new RarityWriteStore(db), new UnitOfWork(db, new TestCurrentUser()));
            return await handler.Handle(new CreateRarityCommand(new CreateRarityPayload(grade, color)), CancellationToken.None);
        }

        private static UpdateRarityHandler UpdateHandler(AppDbContext db)
            => new(new RarityWriteStore(db), new UnitOfWork(db, new TestCurrentUser()));

        private static async Task AssertDtoMatchesEntityAsync(AppDbContext db, RarityReadModel rarityReadModel)
        {
            var saved = await db.Set<Rarity>().AsNoTracking().SingleAsync(r => r.Id == rarityReadModel.Id);
            saved.Grade.Should().Be(rarityReadModel.Grade);
            saved.ColorCode.Should().Be(rarityReadModel.ColorCode);
            saved.RowVersion.Should().Equal(rarityReadModel.RowVersion);
        }
    }
}