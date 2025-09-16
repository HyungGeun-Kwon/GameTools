using FluentAssertions;
using GameTools.Server.Application.Common.Paging;
using GameTools.Server.Application.Features.Audit.Queries.GetItemAuditPage;
using GameTools.Server.Infrastructure.Persistence.Stores.ReadStore;
using GameTools.Server.Test.DataBase.ItemTest;
using GameTools.Server.Test.Utils;
using Microsoft.EntityFrameworkCore;

namespace GameTools.Server.Test.DataBase.AuditTest
{
    public class ItemAuditTests
    {
        [Fact]
        public async Task GetItemAuditPage_Filters_Orders_And_Pages()
        {
            await using var serverDb = await SqlServerTestDb.CreateAsync();
            var db = serverDb.Db;

            // Seed
            var r = serverDb.SeedRarity("COMMON", "#000000");

            // a, b 생성 (INSERT 2건)
            var a = await CreateItemTests.CreateHandler(db)
                .Handle(new Application.Features.Items.Commands.CreateItem.CreateItemCommand(
                    new("A", 10, r.Id, "a")), CancellationToken.None);

            var b = await CreateItemTests.CreateHandler(db)
                .Handle(new Application.Features.Items.Commands.CreateItem.CreateItemCommand(
                    new("B", 20, r.Id, "b")), CancellationToken.None);

            // a 업데이트(UPDATE 1건), b 삭제(DELETE 1건)
            await UpdateItemTests.UpdateItemHandler(db)
                .Handle(new(new(a.Id, "A2", 11, "a2", r.Id, a.RowVersion!)), CancellationToken.None);

            await DeleteItemTest.DeleteHandler(db)
                .Handle(new(new(b.Id, b.RowVersion!)), CancellationToken.None);

            // 총 감사 예상: A: INSERT, UPDATE / B: INSERT, DELETE => 4건
            (await db.ItemAudits.CountAsync()).Should().Be(4);

            var store = new ItemAuditReadStore(db);

            // 1) 전체 조회
            var all = await store.GetItemAuditPageAsync(
                new ItemAuditPageSearchCriteria(
                    new Pagination(1, 50),
                    new ItemAuditFilter(null, null, null, null)), CancellationToken.None);

            all.TotalCount.Should().Be(4);
            all.Items.Should().HaveCount(4);

            // 정렬 검증: ChangedAtUtc desc, AuditId desc
            all.Items.Zip(all.Items.Skip(1)).All(pair =>
                pair.First.ChangedAtUtc >= pair.Second.ChangedAtUtc).Should().BeTrue();

            // 2) Action=UPDATE 필터
            var updates = await store.GetItemAuditPageAsync(
                new ItemAuditPageSearchCriteria(
                    new Pagination(1, 50),
                    new ItemAuditFilter(null, "Update", null, null)), CancellationToken.None);

            updates.TotalCount.Should().Be(1);
            updates.Items.Single().Action.Should().Be("UPDATE");

            // 3) ItemId=a.Id 필터 (a: INSERT + UPDATE = 2)
            var aOnly = await store.GetItemAuditPageAsync(
                new ItemAuditPageSearchCriteria(
                    new Pagination(1, 50),
                    new ItemAuditFilter(a.Id, null, null, null)), CancellationToken.None);

            aOnly.TotalCount.Should().Be(2);
            aOnly.Items.All(x => x.ItemId == a.Id).Should().BeTrue();

            // 4) 기간 필터(최근 1분 내)
            var from = DateTime.UtcNow.AddMinutes(-1);
            var recent = await store.GetItemAuditPageAsync(
                new ItemAuditPageSearchCriteria(
                    new Pagination(1, 50),
                    new ItemAuditFilter(null, null, from, null)), CancellationToken.None);

            recent.TotalCount.Should().Be(4); // 방금 만든 데이터라 전부 매칭
        }
    }
}
