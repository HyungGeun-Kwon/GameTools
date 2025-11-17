using FluentAssertions;
using GameTools.Server.Application.Common.Paging;
using GameTools.Server.Application.Features.Items.Commands.CreateItem;
using GameTools.Server.Application.Features.Restores.Queries.GetRestoreRunsPage;
using GameTools.Server.Infrastructure.Persistence.Stores.ReadStore;
using GameTools.Server.Infrastructure.Persistence.Stores.WriteStore;
using GameTools.Server.Test.DataBase.ItemTest;
using GameTools.Server.Test.Utils;
using Microsoft.EntityFrameworkCore;

namespace GameTools.Server.Test.DataBase.RestoreTest
{
    public class RestoreTest
    {
        [Fact]
        public async Task RestoreRunsPage_After_DryRun_And_Execute_Works_And_Restores_State()
        {
            await using var serverDb = await SqlServerTestDb.CreateAsync();
            var db = serverDb.Db;
            var currentUser = new TestCurrentUser();

            var r = serverDb.SeedRarity("COMMON", "#000000");

            // 1) 아이템 생성
            var created = await CreateItemTests.CreateHandler(db)
                .Handle(new CreateItemCommand(new("RestoreMe", 123, r.Id, "orig")), CancellationToken.None);

            // AsOf 기준 시간(생성 이후 시점으로 고정)
            var asOf = DateTime.UtcNow;

            // 2) 이후 변경(UPDATE 2회)
            var updater = UpdateItemTests.UpdateItemHandler(db);
            var after1 = await updater.Handle(new(new(created.Id, "RestoreMe_v1", 200, "v1", r.Id, created.RowVersion!)), CancellationToken.None);
            var after2 = await updater.Handle(new(new(created.Id, "RestoreMe_v2", 300, "v2", r.Id, after1.ItemReadModel?.RowVersion!)), CancellationToken.None);

            // 현재 DB는 v2 상태
            var v2 = await db.Items.AsNoTracking().SingleAsync(i => i.Id == created.Id);
            v2.Name.Should().Be("RestoreMe_v2");
            v2.Price.Should().Be(300);

            // 3) DRY-RUN 복원
            var writeStore = new ItemWriteStore(db, currentUser);
            var dry = await writeStore.RestoreItemsAsOfAsync(new(asOf, created.Id, true, "dry"), CancellationToken.None);

            dry.Deleted.Should().BeGreaterThanOrEqualTo(0);
            dry.Updated.Should().BeGreaterThan(0); // 최소 1회는 되돌릴 업데이트
            dry.Inserted.Should().BeGreaterThanOrEqualTo(0);

            // 드라이런이므로 DB 상태는 그대로(v2)
            (await db.Items.AsNoTracking().SingleAsync(i => i.Id == created.Id)).Name.Should().Be("RestoreMe_v2");

            // 4) 실제 복원 실행
            var exec = await writeStore.RestoreItemsAsOfAsync(new(asOf, created.Id, false, "exec"), CancellationToken.None);
            exec.Updated.Should().BeGreaterThan(0);

            // 실행 후 DB는 AsOf 당시 상태(= 생성 직후 상태)로 복원되어야 함
            var restored = await db.Items.AsNoTracking().SingleAsync(i => i.Id == created.Id);
            restored.Name.Should().Be("RestoreMe");
            restored.Price.Should().Be(123);
            restored.Description.Should().Be("orig");

            // 5) RestoreRun 읽기 검증
            var readStore = new RestoreRunReadStore(db);

            // 전체 조회: 최소 2건(dry + exec)
            var pageAll = await readStore.GetRestoreRunsPageAsync(
                new RestoreRunPageSearchCriteria(
                    new Pagination(1, 50), 
                    new RestoreRunFilter(asOf.AddMinutes(-5), null, currentUser.UserIdOrName, null)),
                CancellationToken.None);

            pageAll.TotalCount.Should().BeGreaterThanOrEqualTo(2);
            pageAll.Items.Should().NotBeEmpty();

            // 정렬: StartedAtUtc DESC
            pageAll.Items.Zip(pageAll.Items.Skip(1)).All(pair =>
                pair.First.StartedAtUtc >= pair.Second.StartedAtUtc).Should().BeTrue();

            // DryOnly 필터
            var pageDry = await readStore.GetRestoreRunsPageAsync(
                new RestoreRunPageSearchCriteria(
                    new Pagination(1, 50),
                    new RestoreRunFilter(null, null, null, true)), 
                CancellationToken.None);

            pageDry.Items.All(x => x.DryRun).Should().BeTrue();

            var pageExec = await readStore.GetRestoreRunsPageAsync(
                new RestoreRunPageSearchCriteria(
                    new Pagination(1, 50),
                    new RestoreRunFilter(null, null, null, false)), 
                CancellationToken.None);

            pageExec.Items.All(x => x.DryRun == false).Should().BeTrue();

            // AffectedCounts는 문자열 JSON이어야 함(비어있지 않음)
            pageAll.Items[0].AffectedCounts.Should().NotBeNull();
        }
    }
}
