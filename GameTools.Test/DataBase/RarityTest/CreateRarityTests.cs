using FluentAssertions;
using GameTools.Application.Features.Rarities.Commands.CreateRarity;
using GameTools.Application.Features.Rarities.Dtos;
using GameTools.Domain.Auditing;
using GameTools.Domain.Common.Rules;
using GameTools.Domain.Entities;
using GameTools.Infrastructure.Persistence;
using GameTools.Infrastructure.Persistence.Stores.WriteStore;
using GameTools.Infrastructure.Persistence.Works;
using GameTools.Test.Utils;
using Microsoft.EntityFrameworkCore;

namespace GameTools.Test.DataBase.RarityTest
{
    public class CreateRarityTests
    {
        [Fact]
        public async Task CreateRarity_SuccessInMemory()
        {
            await using var db = TestDataBase.CreateTestDbContext();
            var handler = CreateHandler(db);

            var dto = new RarityCreateDto("Common", "#000000");
            var rarityDto = await handler.Handle(new CreateRarityCommand(dto), CancellationToken.None);

            // 반환 Dto 기본 값 검증
            rarityDto.Id.Should().BeGreaterThanOrEqualTo(0);
            rarityDto.Grade.Should().Be("Common");
            rarityDto.ColorCode.Should().Be("#000000");
            rarityDto.RowVersionBase64.Should().NotBeNullOrEmpty();

            // 실제 저장된 엔티티 검증
            var saved = await db.Set<Rarity>().AsNoTracking().SingleAsync(r => r.Id == rarityDto.Id);
            saved.Grade.Should().Be("Common");
            saved.ColorCode.Should().Be("#000000");
            Convert.ToBase64String(saved.RowVersion).Should().Be(rarityDto.RowVersionBase64);
        }

        [Fact]
        public async Task CreateRarity_SuccessSqlServer()
        {
            await using var serverDb = await SqlServerTestDb.CreateAsync();
            var db = serverDb.Db;
            var handler = CreateHandler(db);

            var rarityDto = await handler.Handle(new CreateRarityCommand(new RarityCreateDto("Common", "#000000")), CancellationToken.None);

            // 반환 Dto 기본 값 검증
            rarityDto.Id.Should().BeGreaterThanOrEqualTo(0);
            rarityDto.Grade.Should().Be("Common");
            rarityDto.ColorCode.Should().Be("#000000");
            rarityDto.RowVersionBase64.Should().NotBeNullOrEmpty();

            // 실제 저장된 엔티티 검증
            var saved = await db.Set<Rarity>().AsNoTracking().SingleAsync(r => r.Id == rarityDto.Id);
            saved.Grade.Should().Be("Common");
            saved.ColorCode.Should().Be("#000000");
            Convert.ToBase64String(saved.RowVersion).Should().Be(rarityDto.RowVersionBase64);
        }

        [Fact]
        public async Task CreateRarity_SuccessCreateAuditRarity()
        {
            await using var serverDb = await SqlServerTestDb.CreateAsync();
            var db = serverDb.Db;
            var handler = CreateHandler(db);

            var utcBefore = DateTime.UtcNow;

            var res = await handler.Handle(new CreateRarityCommand(new RarityCreateDto("Common", "#000000")), CancellationToken.None);

            // 저장 확인
            var saved = await db.Set<Rarity>().AsNoTracking().SingleAsync(r => r.Id == res.Id);
            Convert.ToBase64String(saved.RowVersion).Should().Be(res.RowVersionBase64);

            // RarityAudit 확인 (Insert 1건)
            var audits = await db.Set<RarityAudit>().Where(a => a.RarityId == res.Id).ToListAsync();
            audits.Count.Should().Be(1);
            audits[0].Action.Should().Be(AuditAction.Insert);
            audits[0].BeforeJson.Should().BeNullOrEmpty();
            audits[0].AfterJson.Should().NotBeNullOrEmpty();
            audits[0].ChangedBy.Should().Be(new TestCurrentUser().UserIdOrName);
            audits[0].ChangedAtUtc.Should().BeOnOrAfter(utcBefore);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("   ")]
        [InlineData(" \t \r\n ")]
        public async Task CreateRarity_FailWhenGradeNullOrWhitespace(string? grade)
        {
            await using var db = TestDataBase.CreateTestDbContext();
            var handler = CreateHandler(db);

            var act = async () => await handler.Handle(new CreateRarityCommand(new RarityCreateDto(grade, "#000000")), CancellationToken.None);

            await act.Should().ThrowAsync<Exception>();
        }

        [Fact]
        public async Task CreateRarity_GradeMaxLengthBoundary()
        {
            await using var db = TestDataBase.CreateTestDbContext();
            var handler = CreateHandler(db);

            var max = RarityRules.GradeMax;
            var ok = new string('G', max);
            var tooLong = new string('G', max + 1);

            var okRes = await handler.Handle(new CreateRarityCommand(new RarityCreateDto(ok, "#000000")), CancellationToken.None);
            okRes.Grade.Length.Should().Be(max);

            var act = async () => await handler.Handle(new CreateRarityCommand(new RarityCreateDto(tooLong, "#000000")), CancellationToken.None);
            await act.Should().ThrowAsync<Exception>();
        }

        [Theory]
        [InlineData("#000000")]
        [InlineData("#ABCDEF")]
        [InlineData("#123456")]
        [InlineData("#abcDEF")] // 소문자 포함
        public async Task CreateRarity_ColorCodeValidPasses(string color)
        {
            await using var db = TestDataBase.CreateTestDbContext();
            var handler = CreateHandler(db);

            var res = await handler.Handle(new CreateRarityCommand(new RarityCreateDto("COLOR_OK", color)), CancellationToken.None);
            res.ColorCode.ToUpper().Should().Be(color.ToUpper());
        }

        [Theory]
        [InlineData("#12345")] // 길이 짧음
        [InlineData("123456")] // '#' 없음
        [InlineData("#ABCDEG")] // 16진수 아님
        public async Task CreateRarity_ColorCodeInvalidFails(string color)
        {
            await using var serverDb = await SqlServerTestDb.CreateAsync(); // Check 제약은 SQL에서 확실히 검증
            var db = serverDb.Db;
            var handler = CreateHandler(db);

            var act = async () => await handler.Handle(new CreateRarityCommand(new RarityCreateDto("COLOR_NG", color)), CancellationToken.None);
            await act.Should().ThrowAsync<Exception>();
        }

        [Fact]
        public async Task CreateRarity_FailsWhenSameGrade()
        {
            await using var serverDb = await SqlServerTestDb.CreateAsync();
            var db = serverDb.Db;
            var handler = CreateHandler(db);

            await handler.Handle(new CreateRarityCommand(new RarityCreateDto("Common", "#111111")), CancellationToken.None);

            var act = async () => await handler.Handle(new CreateRarityCommand(new RarityCreateDto("Common", "#222222")), CancellationToken.None);
            await act.Should().ThrowAsync<Exception>();
        }

        [Fact]
        public async Task CreateRarity_FailsWhenSameColorCode()
        {
            await using var serverDb = await SqlServerTestDb.CreateAsync();
            var db = serverDb.Db;
            var handler = CreateHandler(db);

            await handler.Handle(new CreateRarityCommand(new RarityCreateDto("Common", "#000000")), default);

            var act = async () => await handler.Handle(new CreateRarityCommand(new RarityCreateDto("Rare", "#000000")), default);
            await act.Should().ThrowAsync<Exception>(); // 유니크 인덱스(ColorCode)
        }

        internal static CreateRarityHandler CreateHandler(AppDbContext db)
            => new(new RarityWriteStore(db), new UnitOfWork(db, new TestCurrentUser()));
    }
}
