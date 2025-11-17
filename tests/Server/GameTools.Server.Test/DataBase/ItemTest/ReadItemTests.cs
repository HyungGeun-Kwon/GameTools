using FluentAssertions;
using GameTools.Server.Application.Common.Paging;
using GameTools.Server.Application.Features.Items.Commands.CreateItem;
using GameTools.Server.Application.Features.Items.Models;
using GameTools.Server.Application.Features.Items.Queries.GetItemById;
using GameTools.Server.Application.Features.Items.Queries.GetItemPage;
using GameTools.Server.Domain.Entities;
using GameTools.Server.Infrastructure.Persistence;
using GameTools.Server.Infrastructure.Persistence.Stores.ReadStore;
using GameTools.Server.Test.Utils;

namespace GameTools.Server.Test.DataBase.ItemTest
{
    public class ReadItemTests
    {
        #region GetItemById
        [Fact]
        public async Task GetItemById()
        {
            await using var serverDb = await SqlServerTestDb.CreateAsync();
            var db = serverDb.Db;

            var rarity = serverDb.SeedRarity(grade: "Common", "#000000");
            var created = await CreateItem(db, "One", 10, rarity.Id, "d");

            var handler = new GetItemByIdHandler(new ItemReadStore(db));
            var itemReadModel = await handler.Handle(new GetItemByIdQuery(created.Id), CancellationToken.None);

            itemReadModel.Should().NotBeNull();
            itemReadModel!.Id.Should().Be(created.Id);
            itemReadModel.Name.Should().Be("One");
            itemReadModel.RarityId.Should().Be(rarity.Id);
            itemReadModel.RarityGrade.Should().Be("Common");
            itemReadModel.RarityColorCode.Should().Be("#000000");
            itemReadModel.RowVersion.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task GetItemById_ReturnsNullWhenNotFound()
        {
            await using var serverDb = await SqlServerTestDb.CreateAsync();
            var db = serverDb.Db;

            var handler = new GetItemByIdHandler(new ItemReadStore(db));
            var itemReadModel = await handler.Handle(new GetItemByIdQuery(999999), CancellationToken.None);

            itemReadModel.Should().BeNull();
        }
        #endregion

        #region GetItemByRarityId
        [Fact]
        public async Task GetItemByRarityId_OnlyMatching()
        {
            await using var serverDb = await SqlServerTestDb.CreateAsync();
            var db = serverDb.Db;

            var ra = serverDb.SeedRarity("Common", "#111111");
            var rb = serverDb.SeedRarity("Rare", "#222222");

            await CreateItem(db, "CommonItem1", 10, ra.Id);
            await CreateItem(db, "CommonItem2", 20, ra.Id);
            await CreateItem(db, "CommonItem3", 30, ra.Id);
            await CreateItem(db, "RareItem1", 40, rb.Id);

            var handler = new GetItemsByRarityIdHandler(new ItemReadStore(db));
            var list = await handler.Handle(new GetItemsByRarityIdQuery(ra.Id), CancellationToken.None);

            list.Count.Should().Be(3);
            list.All(x => x.RarityId == ra.Id).Should().BeTrue();
            list.All(x => !string.IsNullOrEmpty(x.RarityGrade)).Should().BeTrue();
            list.All(x => x.RowVersion?.Length > 0).Should().BeTrue();
        }
        #endregion

        #region GetItemsPage
        [Fact]
        public async Task GetItemsPage_UseAllFilter()
        {
            await using var serverDb = await SqlServerTestDb.CreateAsync();
            var db = serverDb.Db;

            var r1 = serverDb.SeedRarity("Common", "#ABABAB");
            var r2 = serverDb.SeedRarity("Rare", "#CDCDCD");

            await CreateItemsForPageTest(db, r1, r2);

            var filter = new ItemsFilter("Item", r1.Id);
            var itemPageQuery = new GetItemsPageQuery(new ItemsPageSearchCriteria(new Pagination(2, 5), filter));

            var handler = new GetItemsPageHandler(new ItemReadStore(db));
            var pageResult = await handler.Handle(itemPageQuery, CancellationToken.None);

            // r1의 Item* 12개만 카운트
            pageResult.TotalCount.Should().Be(12);
            pageResult.PageNumber.Should().Be(2);
            pageResult.PageSize.Should().Be(5);
            pageResult.TotalPages.Should().Be(3);
            pageResult.HasPrevious.Should().BeTrue();
            pageResult.HasNext.Should().BeTrue();
            pageResult.Items.Should().HaveCount(5);
            pageResult.Items.Select(i => i.Id).Should().BeInDescendingOrder();
            pageResult.Items.All(i => i.RarityId == r1.Id && i.Name.Contains("Item")).Should().BeTrue();
        }

        [Fact]
        public async Task GetItemsPage_UseNameFilter()
        {
            await using var serverDb = await SqlServerTestDb.CreateAsync();
            var db = serverDb.Db;

            var r1 = serverDb.SeedRarity("Common", "#ABABAB");
            var r2 = serverDb.SeedRarity("Rare", "#CDCDCD");

            await CreateItemsForPageTest(db, r1, r2);

            var filter = new ItemsFilter("Item", null);
            var itemPageQuery = new GetItemsPageQuery(new ItemsPageSearchCriteria(new Pagination(2, 5), filter));

            var handler = new GetItemsPageHandler(new ItemReadStore(db));
            var pageResult = await handler.Handle(itemPageQuery, CancellationToken.None);

            // Item* 15개(r1:12 + r2:3) 카운트
            pageResult.TotalCount.Should().Be(15);
            pageResult.PageNumber.Should().Be(2);
            pageResult.PageSize.Should().Be(5);
            pageResult.TotalPages.Should().Be(3);
            pageResult.HasPrevious.Should().BeTrue();
            pageResult.HasNext.Should().BeTrue();
            pageResult.Items.Should().HaveCount(5);
            pageResult.Items.Select(i => i.Id).Should().BeInDescendingOrder();
            pageResult.Items.All(i => i.Name.Contains("Item")).Should().BeTrue();
        }
        [Fact]
        public async Task GetItemsPage_UseRarityFilter()
        {
            await using var serverDb = await SqlServerTestDb.CreateAsync();
            var db = serverDb.Db;

            var r1 = serverDb.SeedRarity("Common", "#ABABAB");
            var r2 = serverDb.SeedRarity("Rare", "#CDCDCD");

            await CreateItemsForPageTest(db, r1, r2);

            var filter = new ItemsFilter(null, r2.Id);
            var itemPageQuery = new GetItemsPageQuery(new ItemsPageSearchCriteria(new Pagination(1, 2), filter));

            var handler = new GetItemsPageHandler(new ItemReadStore(db));
            var pageResult = await handler.Handle(itemPageQuery, CancellationToken.None);

            // r2 사용하는 5개 카운트
            pageResult.TotalCount.Should().Be(5); // r2의 전체 5개
            pageResult.PageNumber.Should().Be(1);
            pageResult.PageSize.Should().Be(2);
            pageResult.TotalPages.Should().Be(3);
            pageResult.HasPrevious.Should().BeFalse();
            pageResult.HasNext.Should().BeTrue();
            pageResult.Items.Should().HaveCount(2);
            pageResult.Items.Select(i => i.Id).Should().BeInDescendingOrder();
            pageResult.Items.All(i => i.RarityId == r2.Id).Should().BeTrue();
        }
        [Fact]
        public async Task GetItemsPage_NoFilter()
        {
            await using var serverDb = await SqlServerTestDb.CreateAsync();
            var db = serverDb.Db;

            var r1 = serverDb.SeedRarity("Common", "#ABABAB");
            var r2 = serverDb.SeedRarity("Rare", "#CDCDCD");

            await CreateItemsForPageTest(db, r1, r2);

            var filter = new ItemsFilter(null, null);
            var itemPageQuery = new GetItemsPageQuery(new ItemsPageSearchCriteria(new Pagination(2, 5), filter));

            var handler = new GetItemsPageHandler(new ItemReadStore(db));
            var pageResult = await handler.Handle(itemPageQuery, CancellationToken.None);

            // 전부 다 Count
            pageResult.TotalCount.Should().Be(17); // 전체
            pageResult.PageNumber.Should().Be(2);
            pageResult.PageSize.Should().Be(5);
            pageResult.TotalPages.Should().Be(4);
            pageResult.HasPrevious.Should().BeTrue();
            pageResult.HasNext.Should().BeTrue();
            pageResult.Items.Should().HaveCount(5);
            pageResult.Items.Select(i => i.Id).Should().BeInDescendingOrder();
        }

        private async static Task CreateItemsForPageTest(AppDbContext db, Rarity r1, Rarity r2)
        {
            // r1: Item01..Item12 (12개), r2: ItemA,B,C(3개) + Other1,2(2개)
            for (int i = 1; i <= 12; i++)
                await CreateItem(db, $"Item{i:00}", i, r1.Id);
            await CreateItem(db, "ItemA", 1, r2.Id);
            await CreateItem(db, "ItemB", 2, r2.Id);
            await CreateItem(db, "ItemC", 3, r2.Id);
            await CreateItem(db, "Other1", 99, r2.Id);
            await CreateItem(db, "Other2", 100, r2.Id);
        }
        #endregion

        private static async Task<ItemReadModel> CreateItem(AppDbContext db, string name, int price, byte rarityId, string? desc = null)
        {
            var handler = CreateItemTests.CreateHandler(db);
            return await handler.Handle(new CreateItemCommand(new CreateItemPayload(name, price, rarityId, desc)), CancellationToken.None);
        }
    }
}
