using FluentAssertions;
using GameTools.Application.Features.Rarities.Commands.CreateRarity;
using GameTools.Application.Features.Rarities.Commands.UpdateRarity;
using GameTools.Application.Features.Rarities.Dtos;
using GameTools.Domain.Auditing;
using GameTools.Domain.Entities;
using GameTools.Infrastructure.Persistence;
using GameTools.Infrastructure.Persistence.Stores.WriteStore;
using GameTools.Infrastructure.Persistence.Works;
using GameTools.Test.Utils;
using Microsoft.EntityFrameworkCore;

namespace GameTools.Test.DataBase.RarityTest
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
                new UpdateRarityCommand(new RarityUpdateDto(
                    created.Id, "Common_Fix", "#111111", created.RowVersionBase64)), default);

            updated.Should().NotBeNull();
            updated.Grade.Should().Be("Common_Fix");
            updated.ColorCode.Should().Be("#111111");
            updated.RowVersionBase64.Should().NotBe(created.RowVersionBase64);

            await AssertDtoMatchesEntityAsync(db, updated);
        }

        [Fact]
        public async Task UpdateRarity_SuccessSqlServer()
        {
            await using var serverDb = await SqlServerTestDb.CreateAsync();
            var db = serverDb.Db;
            var created = await CreateRarity(db, "Common", "#000000");

            var handler = UpdateHandler(db);
            var updated = await handler.Handle(
                new UpdateRarityCommand(new RarityUpdateDto(
                    created.Id, "Common_Fix", "#111111", created.RowVersionBase64)), default);

            updated.Grade.Should().Be("Common_Fix");
            updated.ColorCode.Should().Be("#111111");
            updated.RowVersionBase64.Should().NotBe(created.RowVersionBase64);

            await AssertDtoMatchesEntityAsync(db, updated);
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
                new UpdateRarityCommand(new RarityUpdateDto(
                    created.Id, "Common_Fix", "#111111", created.RowVersionBase64)), default);

            updated.Id.Should().Be(created.Id);

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
                new UpdateRarityCommand(new RarityUpdateDto(
                    rarity2.Id, "Grade_A", rarity2.ColorCode, rarity2.RowVersionBase64)), default);

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
                new UpdateRarityCommand(new RarityUpdateDto(
                    rarity2.Id, rarity2.Grade, "#111111", rarity2.RowVersionBase64)), default);

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
                new UpdateRarityCommand(new RarityUpdateDto(
                    created.Id, created.Grade, created.ColorCode, created.RowVersionBase64)), default);

            // 변경사항이 없다면 RowVersion은 동일해야 함
            updated.RowVersionBase64.Should().Be(created.RowVersionBase64);

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

            var act = async () => await handler.Handle(
                new UpdateRarityCommand(new RarityUpdateDto(
                    250, "None", "#000000", Convert.ToBase64String(new byte[] { 1, 2, 3, 4 }))), default);

            await act.Should().ThrowAsync<Exception>();
        }

        private static async Task<RarityDto> CreateRarity(AppDbContext db, string grade, string color)
        {
            var handler = new CreateRarityHandler(new RarityWriteStore(db), new UnitOfWork(db, new TestCurrentUser()));
            return await handler.Handle(new CreateRarityCommand(new RarityCreateDto(grade, color)), default);
        }

        private static UpdateRarityHandler UpdateHandler(AppDbContext db)
            => new(new RarityWriteStore(db), new UnitOfWork(db, new TestCurrentUser()));

        private static async Task AssertDtoMatchesEntityAsync(AppDbContext db, RarityDto dto)
        {
            var saved = await db.Set<Rarity>().AsNoTracking().SingleAsync(r => r.Id == dto.Id);
            saved.Grade.Should().Be(dto.Grade);
            saved.ColorCode.Should().Be(dto.ColorCode);
            Convert.ToBase64String(saved.RowVersion).Should().Be(dto.RowVersionBase64);
        }
    }
}