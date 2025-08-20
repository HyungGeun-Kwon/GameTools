using FluentAssertions;
using GameTools.Application.Features.Items.Commands.CreateItem;
using GameTools.Application.Features.Items.Dtos;
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
            
            await CreateItemSuccess(db, rarity);
        }

        [Fact]
        public async Task CreateItemSuccessSqlServer()
        {
            await using var serverDb = await SqlServerTestDb.CreateAsync();

            var db = serverDb.Db;
            var rarity = serverDb.SeedRarity();

            await CreateItemSuccess(db, rarity);
        }

        private async Task CreateItemSuccess(AppDbContext db, Rarity rarity)
        {
            var handler = CreateHandler(db);

            var cmd = new CreateItemCommand(new ItemCreateDto("ItemSql", 456, rarity.Id, "SqlServer"));

            var result = await handler.Handle(cmd, CancellationToken.None);

            // 반환 DTO 기본값 검증
            result.Id.Should().BeGreaterThan(0);
            result.Name.Should().Be("ItemSql");
            result.RowVersionBase64.Should().NotBeNullOrEmpty();

            // 실제 저장된 엔티티 검증
            await AssertDtoMatchesEntityAsync(db, result, rarity);
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

        [Fact]
        public async Task CreateItem_FailWhenNameIsNullOrEmpty()
        {
            await using var db = TestDataBase.CreateTestDbContext();
            var handler = CreateHandler(db);

            var rarity = TestDataBase.SeedRarity(db);

            // 이름 null/empty -> 검증 실패.
            var cmdNull = new CreateItemCommand(new ItemCreateDto(null, 100, rarity.Id));
            var cmdEmpty = new CreateItemCommand(new ItemCreateDto("", 100, rarity.Id));

            var actNull = async () => await handler.Handle(cmdNull, CancellationToken.None);
            var actEmpty = async () => await handler.Handle(cmdEmpty, CancellationToken.None);

            await actNull.Should().ThrowAsync<Exception>();
            await actEmpty.Should().ThrowAsync<Exception>();
        }

        [Fact]
        public async Task CreateItem_FailWhenPriceIsNegative()
        {
            await using var db = TestDataBase.CreateTestDbContext();
            var handler = CreateHandler(db);

            var rarity = TestDataBase.SeedRarity(db);

            // 음수 가격 -> 검증 실패.
            var cmd = new CreateItemCommand(new ItemCreateDto("Item1", -1, rarity.Id, null!));
            var act = async () => await handler.Handle(cmd, CancellationToken.None);

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
