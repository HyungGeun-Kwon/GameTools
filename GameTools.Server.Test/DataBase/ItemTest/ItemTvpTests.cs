using FluentAssertions;
using GameTools.Server.Application.Abstractions.Stores.WriteStore;
using GameTools.Server.Application.Features.Items.Commands.CreateItem;
using GameTools.Server.Application.Features.Items.Commands.InsertItemsTvp;
using GameTools.Server.Application.Features.Items.Commands.UpdateItemsTvp;
using GameTools.Server.Domain.Auditing;
using GameTools.Server.Domain.Entities;
using GameTools.Server.Infrastructure.Persistence.Stores.WriteStore;
using GameTools.Server.Infrastructure.Persistence.Works;
using GameTools.Server.Test.Utils;
using Microsoft.EntityFrameworkCore;
using GameTools.Server.Domain.Common.Rules;
using GameTools.Server.Application.Features.Items.Commands.UpdateItem;

namespace GameTools.Server.Test.DataBase.ItemTest
{
    public class ItemTvpTests
    {
        #region InsertTvp
        [Fact]
        public async Task InsertItemsTvp_SuccessReturnsIdsRowVersionsAndCreatesAudits()
        {
            await using var serverDb = await SqlServerTestDb.CreateAsync();
            var db = serverDb.Db;
            var currentUser = new TestCurrentUser();

            var r1 = serverDb.SeedRarity("Common", "#000000");
            var r2 = serverDb.SeedRarity("Rare", "#111111");

            var rows = new List<InsertItemRow>();
            for (int i = 1; i <= 100; i++)
            {
                // r1 10개, r2 90개 생성
                Rarity rairty = r2;
                if (i <= 10) rairty = r1;
                rows.Add(new InsertItemRow($"Item_{i:000}", 100, r1.Id));
            }

            var writeStore = new ItemWriteStore(db, currentUser);
            var handler = new InsertItemsTvpHandler(writeStore);

            var results = await handler.Handle(new InsertItemsTvpCommand(rows), CancellationToken.None);

            results.Should().HaveCount(100);
            results.All(x => x.Id > 0).Should().BeTrue();
            results.All(x => x.RowVersion?.Length > 0).Should().BeTrue();

            // 실제 저장 확인
            var saved = await db.Items.AsNoTracking().Select(i => i).OrderBy(i => i.Id).ToListAsync();
            saved.Should().HaveCount(100);

            // 감사 로그: Insert 100건
            var audits = await db.Set<ItemAudit>().Select(ia => ia).ToListAsync();
            audits.Should().HaveCount(100);
            audits.All(a => a.Action == AuditAction.Insert).Should().BeTrue();
            audits.All(a => a.ChangedBy == currentUser.UserIdOrName).Should().BeTrue();
            audits.All(a => string.IsNullOrEmpty(a.BeforeJson)).Should().BeTrue();
            audits.All(a => !string.IsNullOrEmpty(a.AfterJson)).Should().BeTrue();
        }
        [Fact]
        public async Task InsertItemsTvp_FailsOnSameName()
        {
            await using var serverDb = await SqlServerTestDb.CreateAsync();
            var db = serverDb.Db;

            var r = serverDb.SeedRarity("COMMON", "#AAAAAA");

            // 기본값 생성
            await CreateItemTests.CreateHandler(db).Handle(
                new CreateItemCommand(new CreateItemPayload("Item1", 1, r.Id, null)), CancellationToken.None);

            var rows = new List<InsertItemRow>
            {
                new("Item0", 10, r.Id),
                new("Item1", 20, r.Id, "dup-again"), // Name 유니크 위반
                new("Item2", 30, r.Id),
            };

            var handler = new InsertItemsTvpHandler(new ItemWriteStore(db, new TestCurrentUser()));
            var act = async () => await handler.Handle(new InsertItemsTvpCommand(rows), CancellationToken.None);
            await act.Should().ThrowAsync<Exception>();

            var items = await db.Set<Item>().Select(i => i).ToListAsync();
            items.Count.Should().Be(1); // 최초에 Insert한 1개만 존재
        }

        [Fact]
        public void InsertItemsTvpValidator()
        {
            var v = new InsertItemsTvpValidator();

            // Empty
            v.Validate(new InsertItemsTvpCommand([])).IsValid.Should().BeFalse();

            // Bad fields
            var tooLongName = new string('N', ItemRules.NameMax + 1);
            var tooLongDesc = new string('D', ItemRules.DescriptionMax + 1);

            var cmdOK = new InsertItemsTvpCommand(
            [
                new InsertItemRow("Item1", 10, 1, "") // 정상
            ]);
            var cmdNG1 = new InsertItemsTvpCommand(
            [
                new InsertItemRow("", -1, 0, tooLongDesc) // 전부 위반
            ]);
            var cmdNG2 = new InsertItemsTvpCommand(
            [
                new InsertItemRow(tooLongName, 10, 1, "ok") // 이름 길이 위반
            ]);
            var cmdNG3 = new InsertItemsTvpCommand(
            [
                new InsertItemRow("Item1", 10, 1, tooLongDesc) // 설명 길이 위반
            ]);
            var cmdNG4 = new InsertItemsTvpCommand(
            [
                new InsertItemRow("Item1", -1, 1, "ok") // 가격 위반
            ]);

            v.Validate(cmdOK).IsValid.Should().BeTrue();
            v.Validate(cmdNG1).IsValid.Should().BeFalse();
            v.Validate(cmdNG2).IsValid.Should().BeFalse();
            v.Validate(cmdNG3).IsValid.Should().BeFalse();
            v.Validate(cmdNG4).IsValid.Should().BeFalse();
        }
        #endregion

        #region UpdateTvp
        [Fact]
        public async Task UpdateItemsTvp_SucceedsAndReturnsMixedStatusWithAudits()
        {
            await using var serverDb = await SqlServerTestDb.CreateAsync();
            var db = serverDb.Db;
            var currentUser = new TestCurrentUser();

            var r1 = serverDb.SeedRarity("Common", "#000000");
            var r2 = serverDb.SeedRarity("Rare", "#111111");

            // 기존 2개 생성 (Create 핸들러 사용)
            var creator = CreateItemTests.CreateHandler(db);
            var a = await creator.Handle(new CreateItemCommand(new CreateItemPayload("U-A", 10, r1.Id, "a")), CancellationToken.None);
            var b = await creator.Handle(new CreateItemCommand(new CreateItemPayload("U-B", 20, r2.Id, "b")), CancellationToken.None);

            // b를 선행 업데이트해서 RowVersion을 변경 → TVP에선 stale RowVersion으로 Concurrency 유도
            var uowBump = new UnitOfWork(db, currentUser);
            var entityB = await db.Items.SingleAsync(i => i.Id == b.Id);
            entityB.SetPrice(entityB.Price + 1);
            await uowBump.SaveChangesAsync();

            // TVP 업데이트 rows:
            // - a: 정상 업데이트(Updated, 새 RV)
            // - notFound: 존재하지 않는 Id (NotFound)
            // - b: RowVersion 일치하지 않아 업데이트 실패 (Concurrency)
            var notFoundId = 999999;

            var rows = new List<UpdateItemRow>
            {
                new(a.Id, "U-A2", a.Price + 100, a.Description, a.RarityId, a.RowVersion),
                new(notFoundId, "NF", 1, null, r1.Id, a.RowVersion), // 임의 RV: 존재 여부만 보면 됨
                new(b.Id, "U-B2", b.Price + 200, b.Description, b.RarityId, b.RowVersion), // stale
            };

            var results = await new UpdateItemsTvpHandler(new ItemWriteStore(db, currentUser))
                .Handle(new UpdateItemsTvpCommand(rows), CancellationToken.None);

            results.Should().HaveCount(3);

            var resA = results.Single(r => r.Id == a.Id);
            var resNF = results.Single(r => r.Id == notFoundId);
            var resB = results.Single(r => r.Id == b.Id);

            resA.StatusCode.Should().Be(UpdateStatusCode.Updated);
            resA.NewRowVersion.Should().NotBeNullOrEmpty().And.NotEqual(a.RowVersion);

            resNF.StatusCode.Should().Be(UpdateStatusCode.NotFound);
            resNF.NewRowVersion.Should().BeNull();

            resB.StatusCode.Should().Be(UpdateStatusCode.Concurrency);
            resB.NewRowVersion.Should().BeNull();

            // 실제 DB 반영 확인
            var aSaved = await db.Items.AsNoTracking().SingleAsync(i => i.Id == a.Id);
            aSaved.Name.Should().Be("U-A2");
            aSaved.Price.Should().Be(a.Price + 100);
            aSaved.RowVersion.Should().BeEquivalentTo(resA.NewRowVersion);

            var bSaved = await db.Items.AsNoTracking().SingleAsync(i => i.Id == b.Id);
            bSaved.Name.Should().Be("U-B"); // 변경 실패했으므로 기존 이름 그대로
            bSaved.Price.Should().Be(b.Price + 1); // 선행 bump 반영 상태

            // 감사 로그: a는 Update 1건 추가, b는 실패이므로 추가 없음 (기존값1개), notFound는 대상이 없으니 없음
            var auditsA = await db.Set<ItemAudit>().Where(x => x.ItemId == a.Id && x.Action == AuditAction.Update).ToListAsync();
            auditsA.Should().HaveCount(1);
            auditsA[0].ChangedBy.Should().Be(currentUser.UserIdOrName);
            auditsA[0].BeforeJson.Should().NotBeNullOrEmpty();
            auditsA[0].AfterJson.Should().NotBeNullOrEmpty();

            var auditsB = await db.Set<ItemAudit>().Where(x => x.ItemId == b.Id && x.Action == AuditAction.Update).ToListAsync();
            auditsB.Should().HaveCount(1);
        }

        [Fact]
        public void UpdateItemsTvpValidator_InvalidRowsFails()
        {
            var v = new UpdateItemsTvpValidator();

            // Empty
            v.Validate(new UpdateItemsTvpCommand([])).IsValid.Should().BeFalse();

            // Bad fields
            var tooLongName = new string('N', ItemRules.NameMax + 1);
            var tooLongDesc = new string('D', ItemRules.DescriptionMax + 1);
            var cmdOK = new UpdateItemsTvpCommand(
            [
                new UpdateItemRow(1, "ok", 1, null, 1, [1, 2, 3, 4]), // Id <= 0
            ]);
            var cmdNG1 = new UpdateItemsTvpCommand(
            [
                new UpdateItemRow(0, "ok", 1, null, 1, [1, 2, 3, 4]), // Id <= 0
            ]);
            var cmdNG2 = new UpdateItemsTvpCommand(
            [
                new UpdateItemRow(1, "",  1, null, 1, [1, 2, 3, 4]), // Name empty
            ]);
            var cmdNG3 = new UpdateItemsTvpCommand(
            [
                new UpdateItemRow(1, tooLongName, 1, null, 1, [1, 2, 3, 4]), // Name length
            ]);
            var cmdNG4 = new UpdateItemsTvpCommand(
            [
                new UpdateItemRow(1, "ok", -1, null, 1, [1, 2, 3, 4]), // Price < 0
            ]);
            var cmdNG5 = new UpdateItemsTvpCommand(
            [
                new UpdateItemRow(1, "ok",  1, tooLongDesc, 1, [1, 2, 3, 4]), // Desc length
            ]);
            var cmdNG6 = new UpdateItemsTvpCommand(
            [
                new UpdateItemRow(1, "ok",  1, null, 0, [1, 2, 3, 4]), // RarityId == 0
            ]);
            var cmdNG7 = new UpdateItemsTvpCommand(
            [
                new UpdateItemRow(1, "ok",  1, null, 1, []), // RowVersion empty
            ]);

            v.Validate(cmdOK).IsValid.Should().BeTrue();
            v.Validate(cmdNG1).IsValid.Should().BeFalse();
            v.Validate(cmdNG2).IsValid.Should().BeFalse();
            v.Validate(cmdNG3).IsValid.Should().BeFalse();
            v.Validate(cmdNG4).IsValid.Should().BeFalse();
            v.Validate(cmdNG5).IsValid.Should().BeFalse();
            v.Validate(cmdNG6).IsValid.Should().BeFalse();
            v.Validate(cmdNG7).IsValid.Should().BeFalse();
        }
        #endregion
    }
}
