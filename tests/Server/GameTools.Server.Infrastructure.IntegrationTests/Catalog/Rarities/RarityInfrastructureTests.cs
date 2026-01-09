using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using GameTools.Server.Application.Auditing.Common;
using GameTools.Server.Domain.Catalog.Rarities.Exceptions;
using GameTools.Server.Domain.Catalog.Rarities.Factories;
using GameTools.Server.Domain.Catalog.Rarities.Policies;
using GameTools.Server.Domain.Catalog.Rarities.ValueObjects;
using GameTools.Server.Infrastructure.IntegrationTests.Fixtures;
using GameTools.Server.Infrastructure.Persistence;
using GameTools.Server.Infrastructure.Persistence.Catalog.Checkers;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace GameTools.Server.Infrastructure.IntegrationTests.Catalog.Rarities
{
    [Collection(SqlServerDatabaseCollection.Name)]
    public sealed class RarityInfrastructureTests : IAsyncLifetime
    {
        private readonly SqlServerDatabaseFixture _dbFixture;
        private readonly TestDbContextFactory _dbFactory;

        public RarityInfrastructureTests(SqlServerDatabaseFixture dbFixture)
        {
            _dbFixture = dbFixture;
            _dbFactory = new TestDbContextFactory(_dbFixture.TestDatabaseConnectionString);
        }

        public async Task InitializeAsync()
        {
            await _dbFixture.EnsureMigratedAsync(() => _dbFactory.Create(actor: "Migrator"));
            await using var db = _dbFactory.Create(actor: "Reset");
            await TestDatabaseReset.ResetAsync(db);
        }

        public Task DisposeAsync() => Task.CompletedTask;

        private static RarityFactory CreateFactory_ForGradeUniqueOnly(AppDbContext db)
        {
            var gradeChecker = new RarityGradeUniquenessChecker(db);
            var gradePolicy = new RarityGradeUniquenessPolicy(gradeChecker);

            return new RarityFactory(gradePolicy);
        }

        [Fact]
        public async Task Create_SaveChanges_Writes_Rarity_And_RarityAudit_Insert_With_Actor()
        {
            await using var db = _dbFactory.Create(actor: "Alice");
            await TestDatabaseReset.ResetAsync(db);

            var factory = CreateFactory_ForGradeUniqueOnly(db);

            var rarity = await factory.CreateAsync(
                new RarityGrade("Mythic"),
                new RarityColorCode("#ABCDEF"),
                CancellationToken.None);

            await db.Rarities.AddAsync(rarity);
            await db.SaveChangesAsync();

            // 1) 본 테이블 저장 확인
            var saved = await db.Rarities.AsNoTracking().SingleAsync(r => r.Id == rarity.Id);
            saved.Grade.Value.Should().Be("Mythic");
            saved.ColorCode.Value.Should().Be("#ABCDEF");

            // RowVersion은 shadow property (varbinary(8))
            var rowVersion = await db.Rarities.AsNoTracking()
                .Where(r => r.Id == rarity.Id)
                .Select(r => EF.Property<byte[]>(r, "RowVersion"))
                .SingleAsync();

            rowVersion.Should().NotBeNull();
            rowVersion.Length.Should().Be(8);

            // 2) 트리거로 Audit 적재 확인 (Action/ChangedBy/AfterJson)
            var audit = await db.RarityAudits.AsNoTracking()
                .Where(a => a.RarityId == rarity.Id.Value && a.Action == AuditAction.INSERT)
                .OrderByDescending(a => a.ChangedAtUtc)
                .FirstOrDefaultAsync();

            audit.Should().NotBeNull();
            audit!.ChangedBy.Should().Be("Alice");
            (audit.AfterJson ?? "").Should().Contain("\"Grade\":\"Mythic\"");
            (audit.AfterJson ?? "").Should().Contain("\"ColorCode\":\"#ABCDEF\"");
        }

        [Fact]
        public async Task CreateAsync_Duplicate_Grade_In_Db_Throws_Domain_Exception()
        {
            await using var db = _dbFactory.Create(actor: "Seeder");
            await TestDatabaseReset.ResetAsync(db);

            var factory = CreateFactory_ForGradeUniqueOnly(db);

            // first insert
            var r1 = await factory.CreateAsync(
                new RarityGrade("UniqueGrade"),
                new RarityColorCode("#111111"),
                CancellationToken.None);

            await db.Rarities.AddAsync(r1);
            await db.SaveChangesAsync();

            // second create should fail at policy (DB 조회 기반)
            Func<Task> act = async () =>
            {
                _ = await factory.CreateAsync(
                    new RarityGrade("UniqueGrade"),
                    new RarityColorCode("#222222"),
                    CancellationToken.None);
            };

            await act.Should().ThrowAsync<RarityGradeNotUniqueException>();
        }

        [Fact]
        public async Task Db_CheckConstraint_Blocks_InvalidHex_ColorCode_When_Bypassing_ValueObject()
        {
            await using var db = _dbFactory.Create(actor: "Bypass");
            await TestDatabaseReset.ResetAsync(db);

            Func<Task> act = async () =>
            {
                await db.Database.ExecuteSqlRawAsync("""
                    INSERT INTO dbo.Rarity (Id, Grade, ColorCode)
                    VALUES (NEWID(), N'BadGrade', N'#12ABCG'); -- G 포함: 무조건 실패
                """);
            };

            var ex = await act.Should().ThrowAsync<SqlException>();
            ex.Which.Message.Should().Contain("CK_Rarity_ColorCode_Format");
        }

        [Fact]
        public async Task Concurrency_RowVersion_Throws_DbUpdateConcurrencyException()
        {
            // 같은 데이터를 서로 다른 DbContext 두 개로 동시에 수정
            await using (var seedDb = _dbFactory.Create(actor: "Seeder"))
            {
                await TestDatabaseReset.ResetAsync(seedDb);
                var factory = CreateFactory_ForGradeUniqueOnly(seedDb);

                var rarity = await factory.CreateAsync(
                    new RarityGrade("Before"),
                    new RarityColorCode("#123456"),
                    CancellationToken.None);

                await seedDb.Rarities.AddAsync(rarity);
                await seedDb.SaveChangesAsync();
            }

            await using var db1 = _dbFactory.Create(actor: "User1");
            await using var db2 = _dbFactory.Create(actor: "User2");

            var target1 = await db1.Rarities.SingleAsync(r => r.Grade == new RarityGrade("Before"));
            var target2 = await db2.Rarities.SingleAsync(r => r.Grade == new RarityGrade("Before"));

            target1.ChangeGrade(new RarityGrade("After-1"));
            await db1.SaveChangesAsync(); // 성공

            target2.ChangeGrade(new RarityGrade("After-2"));

            Func<Task> act = () => db2.SaveChangesAsync();
            await act.Should().ThrowAsync<DbUpdateConcurrencyException>(); // RowVersion 충돌
        }
    }
}
