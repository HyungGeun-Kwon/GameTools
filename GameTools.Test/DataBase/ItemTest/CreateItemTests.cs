using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using GameTools.Application.Features.Items.Commands.CreateItem;
using GameTools.Application.Features.Items.Dtos;
using GameTools.Domain.Entities;
using GameTools.Infrastructure.Persistence.Stores.WriteStore;
using GameTools.Infrastructure.Persistence.Works;
using GameTools.Test.Utils;
using Microsoft.EntityFrameworkCore;

namespace GameTools.Test.DataBase.ItemTest
{
    public class CreateItemTests
    {
        [Fact]
        public async Task CreateItem_PersistsAndReturnsDto()
        {
            await using var db = TestDataBase.CreateTestDbContext();
            var currentUser = new TestCurrentUser();
            var handler = new CreateItemHandler(
                new ItemWriteStore(db, currentUser),
                new RarityWriteStore(db),
                new UnitOfWork(db, currentUser)
            );

            Rarity rarity = TestDataBase.SeedRarity(db);
            var command = new CreateItemCommand(new ItemCreateDto("Item1", 100, rarity.Id, "First Item"));

            var result = await handler.Handle(command, CancellationToken.None);

            result.Id.Should().BeGreaterThan(0);
            result.Name.Should().Be("Item1");
            result.RowVersionBase64.Should().NotBeNullOrEmpty();
            result.Should().BeEquivalentTo(
                new ItemDto(
                    result.Id,
                    command.ItemCreateDto.Name,
                    command.ItemCreateDto.Price,
                    command.ItemCreateDto.Description,
                    rarity.Id,
                    rarity.Grade,
                    rarity.ColorCode,
                    result.RowVersionBase64
                )
            );

            Item? saved = await db.Set<Item>()
                .AsNoTracking()
                .FirstOrDefaultAsync(i => i.Id == result.Id);

            saved.Should().NotBeNull();
            saved!.Id.Should().Be(result.Id);
            saved.Name.Should().Be(result.Name);
            saved.Price.Should().Be(result.Price);
            saved.Description.Should().Be(result.Description);
            saved.RarityId.Should().Be(rarity.Id);
            Convert.ToBase64String(saved.RowVersion).Should().Be(result.RowVersionBase64);
        }

        [Fact]
        public async Task CreateItem_AllowsNullDescription()
        {
            await using var db = TestDataBase.CreateTestDbContext();
            var currentUser = new TestCurrentUser();
            var handler = new CreateItemHandler(
                new ItemWriteStore(db, currentUser),
                new RarityWriteStore(db),
                new UnitOfWork(db, currentUser)
            );

            var rarity = TestDataBase.SeedRarity(db);
            var cmd = new CreateItemCommand(new ItemCreateDto("ItemNullDesc", 10, rarity.Id, null));

            var result = await handler.Handle(cmd, CancellationToken.None);

            result.Description.Should().BeNull();

            var saved = await db.Set<Item>()
                .AsNoTracking()
                .FirstAsync(i => i.Id == result.Id);

            saved.Description.Should().BeNull();
        }

        [Fact]
        public async Task CreateItem_FailWhenNameIsNullOrEmpty()
        {
            await using var db = TestDataBase.CreateTestDbContext();
            var currentUser = new TestCurrentUser();
            var handler = new CreateItemHandler(
                new ItemWriteStore(db, currentUser),
                new RarityWriteStore(db),
                new UnitOfWork(db, currentUser)
            );

            var rarity = TestDataBase.SeedRarity(db);

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
            var currentUser = new TestCurrentUser();
            var handler = new CreateItemHandler(
                new ItemWriteStore(db, currentUser),
                new RarityWriteStore(db),
                new UnitOfWork(db, currentUser)
            );

            var rarity = TestDataBase.SeedRarity(db);

            var cmd = new CreateItemCommand(new ItemCreateDto("Item1", -1, rarity.Id, null!));
            var act = async () => await handler.Handle(cmd, CancellationToken.None);

            await act.Should().ThrowAsync<Exception>();
        }

        [Fact]
        public async Task CreateItem_FailWhenRarityNotFound()
        {
            await using var db = TestDataBase.CreateTestDbContext();
            var currentUser = new TestCurrentUser();

            var itemWrite = new ItemWriteStore(db, currentUser);
            var rarityWrite = new RarityWriteStore(db);
            var uow = new UnitOfWork(db, currentUser);

            var handler = new CreateItemHandler(itemWrite, rarityWrite, uow);

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
            var currentUser = new TestCurrentUser();
            var handler = new CreateItemHandler(
                new ItemWriteStore(db, currentUser),
                new RarityWriteStore(db),
                new UnitOfWork(db, currentUser)
            );

            var rarity = TestDataBase.SeedRarity(db);
            var cmd = new CreateItemCommand(new ItemCreateDto("Item1", 100, rarity.Id, "First"));

            var result = await handler.Handle(cmd, CancellationToken.None);

            var saved = await db.Set<Item>()
                .AsNoTracking()
                .FirstAsync(i => i.Id == result.Id);

            Convert.ToBase64String(saved.RowVersion).Should().Be(result.RowVersionBase64);
        }

        [Fact]
        public async Task CreateItem_FailsSameNameCreate()
        {
            await using var serverDb = await SqlServerTestDb.CreateAsync();

            var db = serverDb.Db;
            var currentUser = new TestCurrentUser();
            var handler = new CreateItemHandler(
                new ItemWriteStore(db, currentUser),
                new RarityWriteStore(db),
                new UnitOfWork(db, currentUser)
            );

            var rarity = serverDb.SeedRarity();

            var cmd = new CreateItemCommand(new ItemCreateDto("Item1", 100, rarity.Id, "First"));
            
            var create1 = await handler.Handle(cmd, CancellationToken.None);
            var act = async () => await handler.Handle(cmd, CancellationToken.None);

            await act.Should().ThrowAsync<Exception>();
        }
    }
}
