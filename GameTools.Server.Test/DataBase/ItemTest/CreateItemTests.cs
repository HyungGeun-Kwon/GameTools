using FluentAssertions;
using GameTools.Server.Application.Features.Items.Commands.CreateItem;
using GameTools.Server.Application.Features.Items.Models;
using GameTools.Server.Domain.Auditing;
using GameTools.Server.Domain.Common.Rules;
using GameTools.Server.Domain.Entities;
using GameTools.Server.Infrastructure.Persistence;
using GameTools.Server.Infrastructure.Persistence.Stores.WriteStore;
using GameTools.Server.Infrastructure.Persistence.Works;
using GameTools.Server.Test.Utils;
using Microsoft.EntityFrameworkCore;

namespace GameTools.Server.Test.DataBase.ItemTest
{
    public class CreateItemTests
    {
        [Fact]
        public async Task CreateItem_SuccessInMemory()
        {
            await using var db = TestDataBase.CreateTestDbContext();

            var rarity = TestDataBase.SeedRarity(db);

            var createItemPayload = new CreateItemPayload("ItemInMem", 456, rarity.Id, "SqlServer");

            var _ = await CreateItem(db, createItemPayload, rarity);
        }

        [Fact]
        public async Task CreateItem_SuccessSqlServer()
        {
            await using var serverDb = await SqlServerTestDb.CreateAsync();

            var db = serverDb.Db;
            var rarity = serverDb.SeedRarity();

            var createItemPayload = new CreateItemPayload("ItemSql", 456, rarity.Id, "SqlServer");

            var _ = await CreateItem(db, createItemPayload, rarity);
        }

        [Fact]
        public async Task CreateItem_SuccessCreateAuditItem()
        {
            await using var serverDb = await SqlServerTestDb.CreateAsync();

            var db = serverDb.Db;
            var rarity = serverDb.SeedRarity();

            var createItemPayload = new CreateItemPayload("item1", 456, rarity.Id, "Firsts");

            var utcBeforeCreate = DateTime.UtcNow;
            ItemReadModel itemReadModel = await CreateItem(db, createItemPayload, rarity);

            var itemaudits = await db.Set<ItemAudit>().Select(ia => ia).ToListAsync();

            itemaudits.Count.Should().Be(1);
            itemaudits[0].AuditId.Should().BeGreaterThan(0);
            itemaudits[0].Action.Should().Be(AuditAction.Insert);
            itemaudits[0].ItemId.Should().Be(itemReadModel.Id);
            itemaudits[0].BeforeJson.Should().BeNullOrEmpty();
            itemaudits[0].AfterJson.Should().NotBeNullOrEmpty();
            itemaudits[0].ChangedBy.Should().Be(new TestCurrentUser().UserIdOrName);
            itemaudits[0].ChangedAtUtc.Should().BeOnOrAfter(utcBeforeCreate);
        }

        [Fact]
        public async Task CreateItem_AllowsNullDescription()
        {
            await using var db = TestDataBase.CreateTestDbContext();
            var handler = CreateHandler(db);

            var rarity = TestDataBase.SeedRarity(db);
            // 설명을 null로 생성 요청
            var cmd = new CreateItemCommand(new CreateItemPayload("ItemNullDesc", 10, rarity.Id, null));

            var result = await handler.Handle(cmd, CancellationToken.None);

            // payload와 실제 저장값이 둘 다 null인지 확인
            result.Description.Should().BeNull();
            var saved = await GetSavedAsync(db, result.Id);
            saved.Description.Should().BeNull();
        }

        [Theory]
        [InlineData(0, true)]
        [InlineData(-1, false)]
        public async Task CreateItem_PriceBoundary(int price, bool shouldSucceed)
        {
            await using var db = TestDataBase.CreateTestDbContext();
            var handler = CreateHandler(db);

            var rarity = TestDataBase.SeedRarity(db);

            // 음수 가격 -> 검증 실패.
            var cmd = new CreateItemCommand(new CreateItemPayload("Item1", price, rarity.Id, null!));
            var act = async () => await handler.Handle(cmd, CancellationToken.None);

            if (shouldSucceed) await act.Should().NotThrowAsync();
            else await act.Should().ThrowAsync<Exception>();
        }

        [Fact]
        public async Task CreateItem_NameMaxLengthBoundary()
        {
            await using var db = TestDataBase.CreateTestDbContext();
            var handler = CreateHandler(db);
            var rarity = TestDataBase.SeedRarity(db);

            var max = ItemRules.NameMax;
            var ok = new string('A', max);
            var tooLong = new string('A', max + 1);

            // OK
            var okPayload = new CreateItemPayload(ok, 10, rarity.Id, null);
            ItemReadModel okReadModel = await handler.Handle(new CreateItemCommand(okPayload), CancellationToken.None);
            okReadModel.Name.Length.Should().Be(max);

            // Fail
            var padPayload = new CreateItemPayload(tooLong, 10, rarity.Id, null);
            var act = async () => await handler.Handle(new CreateItemCommand(padPayload), CancellationToken.None);
            await act.Should().ThrowAsync<Exception>();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("   ")]
        [InlineData(" \t \r\n ")]
        public async Task CreateItem_FailWhenNameNullOrWhitespace(string? name)
        {
            await using var db = TestDataBase.CreateTestDbContext();
            var handler = CreateHandler(db);
            var rarity = TestDataBase.SeedRarity(db);

            var act = async () => await handler.Handle(new CreateItemCommand(new CreateItemPayload(name, 1, rarity.Id)), CancellationToken.None);
            await act.Should().ThrowAsync<Exception>();
        }

        [Fact]
        public async Task CreateItem_DescriptionMaxLengthBoundary()
        {
            await using var db = TestDataBase.CreateTestDbContext();
            var handler = CreateHandler(db);
            var rarity = TestDataBase.SeedRarity(db);

            var max = ItemRules.DescriptionMax;
            var ok = new string('D', max);
            var tooLong = new string('D', max + 1);

            await handler.Handle(new CreateItemCommand(new CreateItemPayload("Item1", 1, rarity.Id, ok)), CancellationToken.None);

            var act = async () => await handler.Handle(
                new CreateItemCommand(new CreateItemPayload("DescTooLong", 1, rarity.Id, tooLong)), CancellationToken.None);
            await act.Should().ThrowAsync<Exception>();
        }

        [Fact]
        public async Task CreateItem_FailWhenRarityNotFound()
        {
            await using var db = TestDataBase.CreateTestDbContext();
            var handler = CreateHandler(db);

            // 존재하지 않는 RarityId로 생성 -> 검증 실패.
            var act = async () => await handler.Handle(
                new CreateItemCommand(new CreateItemPayload("Item1", 500, 250, null)),
                CancellationToken.None
            );

            await act.Should().ThrowAsync<Exception>();
        }

        [Fact]
        public async Task CreateItem_PersistsRowVersionAndPayloadShouldMatch()
        {
            await using var db = TestDataBase.CreateTestDbContext();
            var handler = CreateHandler(db);

            var rarity = TestDataBase.SeedRarity(db);

            var cmd = new CreateItemCommand(new CreateItemPayload("Item1", 100, rarity.Id, "First"));

            var result = await handler.Handle(cmd, CancellationToken.None);

            // 저장된 RowVersion 과 Payload의 Base64 값 일치 확인
            var saved = await GetSavedAsync(db, result.Id);
            saved.RowVersion.Should().Equal(result.RowVersion);
        }

        [Fact]
        public async Task CreateItem_FailsSameNameCreate()
        {
            // 유니크 인덱스 검증을 위해 실제 관계형 DB 사용
            await using var serverDb = await SqlServerTestDb.CreateAsync();

            var db = serverDb.Db;
            var handler = CreateHandler(db);

            var rarity = serverDb.SeedRarity();

            // 같은 이름 두 번 생성 -> 유니크 인덱스 위반
            var cmd = new CreateItemCommand(new CreateItemPayload("Item1", 100, rarity.Id, "First"));
            
            var create = await handler.Handle(cmd, CancellationToken.None);
            var act = async () => await handler.Handle(cmd, CancellationToken.None);

            await act.Should().ThrowAsync<Exception>();
        }

        // 핸들러 조립 헬퍼
        internal static CreateItemHandler CreateHandler(AppDbContext db)
            => new(new ItemWriteStore(db, new TestCurrentUser()), new RarityWriteStore(db), new UnitOfWork(db, new TestCurrentUser()));

        // 저장된 엔티티 로드 헬퍼
        internal static async Task<Item> GetSavedAsync(AppDbContext db, int id)
            => await db.Set<Item>().AsNoTracking().SingleAsync(i => i.Id == id);

        // Item 생성 헬퍼
        private async Task<ItemReadModel> CreateItem(AppDbContext db, CreateItemPayload createItempayload, Rarity rarity)
        {
            var handler = CreateHandler(db);

            var cmd = new CreateItemCommand(createItempayload);

            var result = await handler.Handle(cmd, CancellationToken.None);

            // 반환 기본값 검증
            result.Id.Should().BeGreaterThan(0);
            result.Name.Should().Be(createItempayload.Name);
            result.RowVersion.Should().NotBeNullOrEmpty();

            // 실제 저장된 엔티티 검증
            await AssertReadModelMatchesEntityAsync(db, result, rarity);

            return result;
        }

        // ReadModel / 저장 엔티티 일치 검증 헬퍼
        private static async Task AssertReadModelMatchesEntityAsync(AppDbContext db, ItemReadModel itemReadModel, Rarity rarity)
        {
            // ReadModel 기본 스펙 검증
            itemReadModel.Id.Should().BeGreaterThan(0);
            itemReadModel.RowVersion.Should().NotBeNullOrEmpty();

            // 실제 저장된 엔티티 검증
            var saved = await GetSavedAsync(db, itemReadModel.Id);
            saved.Id.Should().Be(itemReadModel.Id);
            saved.Name.Should().Be(itemReadModel.Name);
            saved.Price.Should().Be(itemReadModel.Price);
            saved.Description.Should().Be(itemReadModel.Description);
            saved.RarityId.Should().Be(rarity.Id);
        }
    }
}
