using FluentAssertions;
using GameTools.Application.Features.Items.Commands.CreateItem;
using GameTools.Application.Features.Items.Dtos;
using GameTools.Domain.Auditing;
using GameTools.Domain.Common.Rules;
using GameTools.Domain.Entities;
using GameTools.Infrastructure.Persistence;
using GameTools.Infrastructure.Persistence.Stores.WriteStore;
using GameTools.Infrastructure.Persistence.Works;
using GameTools.Test.Utils;
using Microsoft.EntityFrameworkCore;

namespace GameTools.Test.DataBase.ItemTest
{
    // TODO : Audit값 추가되는지 확인해줘야함
    public class CreateItemTests
    {
        [Fact]
        public async Task CreateItemSuccessInMemory()
        {
            await using var db = TestDataBase.CreateTestDbContext();

            var rarity = TestDataBase.SeedRarity(db);

            var itemCreateDto = new ItemCreateDto("ItemInMem", 456, rarity.Id, "SqlServer");

            var _ = await CreateItem(db, itemCreateDto, rarity);
        }

        [Fact]
        public async Task CreateItemSuccessSqlServer()
        {
            await using var serverDb = await SqlServerTestDb.CreateAsync();

            var db = serverDb.Db;
            var rarity = serverDb.SeedRarity();

            var itemCreateDto = new ItemCreateDto("ItemSql", 456, rarity.Id, "SqlServer");

            var utcNow = DateTime.UtcNow;
            ItemDto itemDto = await CreateItem(db, itemCreateDto, rarity);

            var itemaudits = await db.Set<ItemAudit>().Select(ia => ia).ToListAsync();

            itemaudits.Count.Should().Be(1);
            itemaudits[0].AuditId.Should().BeGreaterThan(0);
            itemaudits[0].Action.Should().Be(AuditAction.Insert);
            itemaudits[0].ItemId.Should().Be(itemDto.Id);
            itemaudits[0].BeforeJson.Should().BeNullOrEmpty();
            itemaudits[0].AfterJson.Should().NotBeNullOrEmpty();
            itemaudits[0].ChangedBy.Should().Be(new TestCurrentUser().UserIdOrName);
            itemaudits[0].ChangedAtUtc.Should().BeOnOrAfter(utcNow);
        }

        private async Task<ItemDto> CreateItem(AppDbContext db, ItemCreateDto itemCreateDto, Rarity rarity)
        {
            var handler = CreateHandler(db);

            var cmd = new CreateItemCommand(itemCreateDto);

            var result = await handler.Handle(cmd, CancellationToken.None);

            // 반환 DTO 기본값 검증
            result.Id.Should().BeGreaterThan(0);
            result.Name.Should().Be(itemCreateDto.Name);
            result.RowVersionBase64.Should().NotBeNullOrEmpty();

            // 실제 저장된 엔티티 검증
            await AssertDtoMatchesEntityAsync(db, result, rarity);

            return result;
        }

        [Fact]
        public async Task CreateItem_AllowsNullDescription()
        {
            await using var db = TestDataBase.CreateTestDbContext();
            var handler = CreateHandler(db);

            var rarity = TestDataBase.SeedRarity(db);
            // 설명을 null로 생성 요청
            var cmd = new CreateItemCommand(new ItemCreateDto("ItemNullDesc", 10, rarity.Id, null));

            var result = await handler.Handle(cmd, CancellationToken.None);

            // DTO와 실제 저장값이 둘 다 null인지 확인
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
            var cmd = new CreateItemCommand(new ItemCreateDto("Item1", price, rarity.Id, null!));
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
            var okDto = new ItemCreateDto(ok, 10, rarity.Id, null);
            var okRes = await handler.Handle(new CreateItemCommand(okDto), default);
            okRes.Name.Length.Should().Be(max);

            // Fail
            var badDto = new ItemCreateDto(tooLong, 10, rarity.Id, null);
            var act = async () => await handler.Handle(new CreateItemCommand(badDto), default);
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

            var act = async () => await handler.Handle(new CreateItemCommand(new ItemCreateDto(name, 1, rarity.Id)), default);
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

            await handler.Handle(new CreateItemCommand(new ItemCreateDto("Item1", 1, rarity.Id, ok)), default);

            var act = async () => await handler.Handle(
                new CreateItemCommand(new ItemCreateDto("DescTooLong", 1, rarity.Id, tooLong)), default);
            await act.Should().ThrowAsync<Exception>();
        }

        [Fact]
        public async Task CreateItem_FailWhenRarityNotFound()
        {
            await using var db = TestDataBase.CreateTestDbContext();
            var handler = CreateHandler(db);

            // 존재하지 않는 RarityId로 생성 -> 검증 실패.
            var act = async () => await handler.Handle(
                new CreateItemCommand(new ItemCreateDto("Item1", 500, 250, null)),
                default
            );

            await act.Should().ThrowAsync<Exception>();
        }

        [Fact]
        public async Task CreateItem_PersistsRowVersionAndDtoBase64ShouldMatch()
        {
            await using var db = TestDataBase.CreateTestDbContext();
            var handler = CreateHandler(db);

            var rarity = TestDataBase.SeedRarity(db);

            var cmd = new CreateItemCommand(new ItemCreateDto("Item1", 100, rarity.Id, "First"));

            var result = await handler.Handle(cmd, CancellationToken.None);

            // 저장된 RowVersion 과 DTO의 Base64 값 일치 확인
            var saved = await GetSavedAsync(db, result.Id);
            Convert.ToBase64String(saved.RowVersion).Should().Be(result.RowVersionBase64);
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
            var cmd = new CreateItemCommand(new ItemCreateDto("Item1", 100, rarity.Id, "First"));
            
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

        // DTO / 저장 엔티티 일치 검증 헬퍼
        private static async Task AssertDtoMatchesEntityAsync(AppDbContext db, ItemDto dto, Rarity rarity)
        {
            // DTO 기본 스펙 검증
            dto.Id.Should().BeGreaterThan(0);
            dto.RowVersionBase64.Should().NotBeNullOrEmpty();

            // 실제 저장된 엔티티 검증
            var saved = await GetSavedAsync(db, dto.Id);
            saved.Id.Should().Be(dto.Id);
            saved.Name.Should().Be(dto.Name);
            saved.Price.Should().Be(dto.Price);
            saved.Description.Should().Be(dto.Description);
            saved.RarityId.Should().Be(rarity.Id);

            // RowVersion(Base64) 일치 검증
            Convert.ToBase64String(saved.RowVersion).Should().Be(dto.RowVersionBase64);
        }
    }
}
