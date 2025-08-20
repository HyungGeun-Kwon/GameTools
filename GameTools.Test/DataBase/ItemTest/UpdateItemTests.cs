using FluentAssertions;
using GameTools.Application.Features.Items.Commands.CreateItem;
using GameTools.Application.Features.Items.Commands.UpdateItem;
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
    public class UpdateItemTests
    {
        [Fact]
        public async Task UpdateItem_SuccessInMemory()
        {
            await using var db = TestDataBase.CreateTestDbContext();
            await UpdateItemSuccess(db);
        }

        [Fact]
        public async Task UpdateItem_SuccessSqlServer()
        {
            await using var serverDb = await SqlServerTestDb.CreateAsync();
            await UpdateItemSuccess(serverDb.Db);
        }

        [Fact]
        public async Task UpdateItem_FailsRowVersionMismatch()
        {
            await using var serverDb = await SqlServerTestDb.CreateAsync();

            var db = serverDb.Db;
            var currentUser = new TestCurrentUser();
            var handler = CreateItemTests.CreateHandler(db);

            var rarity = serverDb.SeedRarity();

            // 아이템 생성
            var cmd = new CreateItemCommand(new ItemCreateDto("Item1", 100, rarity.Id, "First"));
            var create = await handler.Handle(cmd, CancellationToken.None);

            // 서로 다른 컨텍스트 두 개 준비
            await using var db1 = serverDb.NewDb();
            await using var db2 = serverDb.NewDb();

            // 각각의 컨텍스트에서 아이템 조회
            var item1 = await db1.Items.SingleAsync(i => i.Id == create.Id);
            var item2 = await db2.Items.SingleAsync(i => i.Id == create.Id);

            // 가격 수정
            item1.SetPrice(item1.Price + 100);
            item2.SetPrice(item1.Price + 200);

            // 첫번째 컨텍스트에서 업데이트
            var uow1 = new UnitOfWork(db1, currentUser);
            await uow1.SaveChangesAsync();

            // 두번째 컨텍스트에서 업데이트
            var uow2 = new UnitOfWork(db2, currentUser);
            var act = async () => await uow2.SaveChangesAsync();

            await act.Should().ThrowAsync<Exception>();
        }

        private static async Task UpdateItemSuccess(AppDbContext db)
        {
            var createdItem = await CreateItem(db);

            var handler = UpdateItemHandler(db);
            var updatedItem = await handler.Handle(
                new UpdateItemCommand(new ItemUpdateDto(
                    createdItem.Id,
                    createdItem.Name,
                    createdItem.Price + 100,
                    createdItem.Description,
                    createdItem.RarityId,
                    createdItem.RowVersionBase64)), CancellationToken.None);


            // 반환 DTO 기본값 검증
            updatedItem.Should().NotBeNull();
            updatedItem.Price.Should().Be(createdItem.Price + 100);

            // RowVersion변경 확인
            updatedItem.RowVersionBase64.Should().NotBe(createdItem.RowVersionBase64);

            await AssertDtoMatchesEntityAsync(db, updatedItem);
        }

        private static async Task<ItemDto> CreateItem(AppDbContext db)
        {
            var currentUser = new TestCurrentUser();
            var handler = CreateItemTests.CreateHandler(db);
            Rarity rarity = TestDataBase.SeedRarity(db);
            var cmd = new CreateItemCommand(new ItemCreateDto("Item1", 100, rarity.Id, "First Item"));
            return await handler.Handle(cmd, CancellationToken.None);
        }

        private static UpdateItemHandler UpdateItemHandler(AppDbContext db)
            => new(
                new ItemWriteStore(db, new TestCurrentUser()),
                new RarityWriteStore(db),
                new UnitOfWork(db, new TestCurrentUser()));

        // DTO / 저장 엔티티 일치 검증 헬퍼
        private static async Task AssertDtoMatchesEntityAsync(AppDbContext db, ItemDto dto)
        {
            // 실제 저장된 엔티티 검증
            var saved = await CreateItemTests.GetSavedAsync(db, dto.Id);

            saved.Id.Should().Be(dto.Id);
            saved.Name.Should().Be(dto.Name);
            saved.Price.Should().Be(dto.Price);
            saved.Description.Should().Be(dto.Description);
            saved.RarityId.Should().Be(dto.RarityId);

            // RowVersion(Base64) 일치 검증
            Convert.ToBase64String(saved.RowVersion).Should().Be(dto.RowVersionBase64);
        }
    }
}