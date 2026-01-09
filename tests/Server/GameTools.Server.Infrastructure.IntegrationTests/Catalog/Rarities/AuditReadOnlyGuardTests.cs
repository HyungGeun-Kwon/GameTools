using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using GameTools.Server.Domain.Catalog.Rarities.Factories;
using GameTools.Server.Domain.Catalog.Rarities.Policies;
using GameTools.Server.Infrastructure.IntegrationTests.Fixtures;
using GameTools.Server.Infrastructure.Persistence;
using GameTools.Server.Infrastructure.Persistence.Auditing.Models;
using GameTools.Server.Infrastructure.Persistence.Catalog.Checkers;
using Microsoft.EntityFrameworkCore;

namespace GameTools.Server.Infrastructure.IntegrationTests.Catalog.Rarities
{
    [Collection(SqlServerDatabaseCollection.Name)]
    public sealed class AuditReadOnlyGuardTests : IAsyncLifetime
    {
        private readonly SqlServerDatabaseFixture _dbFixture;
        private readonly TestDbContextFactory _dbFactory;

        public AuditReadOnlyGuardTests(SqlServerDatabaseFixture dbFixture)
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
        public async Task Audit_Added_Then_SaveChanges_Throws_ReadOnly_Guard()
        {
            await using var db = _dbFactory.Create(actor: "TestUser");
            await TestDatabaseReset.ResetAsync(db);

            // RarityAudit는 private ctor -> 리플렉션으로 생성
            var audit = (RarityAudit)Activator.CreateInstance(typeof(RarityAudit), nonPublic: true)!;

            db.RarityAudits.Add(audit); // State = Added

            Func<Task> act = () => db.SaveChangesAsync();

            var ex = await act.Should().ThrowAsync<InvalidOperationException>();
            ex.Which.Message.Should().Contain("Audit tables are read-only");
        }

        [Fact]
        public async Task Audit_Modified_Then_SaveChanges_Throws_ReadOnly_Guard()
        {
            await using var db = _dbFactory.Create(actor: "Seeder");
            await TestDatabaseReset.ResetAsync(db);

            var factory = CreateFactory_ForGradeUniqueOnly(db);

            // 1) Rarity 저장 -> 트리거가 RarityAudit 생성
            var rarity = await factory.CreateAsync(
                grade: new("Common"),
                colorCode: new("#A0A0A0"),
                CancellationToken.None);

            db.Rarities.Add(rarity);
            await db.SaveChangesAsync();

            // 2) 생성된 audit 로드 후 상태를 Modified로 강제
            var audit = await db.RarityAudits
                .OrderByDescending(a => a.ChangedAtUtc)
                .FirstAsync();

            db.Entry(audit).State = EntityState.Modified;

            Func<Task> act = () => db.SaveChangesAsync();

            var ex = await act.Should().ThrowAsync<InvalidOperationException>();
            ex.Which.Message.Should().Contain("Audit tables are read-only");
        }

        [Fact]
        public async Task Audit_Deleted_Then_SaveChanges_Throws_ReadOnly_Guard()
        {
            await using var db = _dbFactory.Create(actor: "Seeder");
            await TestDatabaseReset.ResetAsync(db);

            var factory = CreateFactory_ForGradeUniqueOnly(db);

            var rarity = await factory.CreateAsync(
                grade: new("Common"),
                colorCode: new("#A0A0A0"),
                CancellationToken.None);

            db.Rarities.Add(rarity);
            await db.SaveChangesAsync();

            var audit = await db.RarityAudits
                .OrderByDescending(a => a.ChangedAtUtc)
                .FirstAsync();

            db.RarityAudits.Remove(audit); // State = Deleted

            Func<Task> act = () => db.SaveChangesAsync();

            var ex = await act.Should().ThrowAsync<InvalidOperationException>();
            ex.Which.Message.Should().Contain("Audit tables are read-only");
        }
    }
}
