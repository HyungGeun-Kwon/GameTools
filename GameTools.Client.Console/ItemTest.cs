using System.Diagnostics;
using GameTools.Client.Application.Common.Paging;
using GameTools.Client.Application.UseCases.Items.BulkInsertItems;
using GameTools.Client.Application.UseCases.Items.BulkUpdateItems;
using GameTools.Client.Application.UseCases.Items.CreateItem;
using GameTools.Client.Application.UseCases.Items.DeleteItem;
using GameTools.Client.Application.UseCases.Items.GetItemById;
using GameTools.Client.Application.UseCases.Items.GetItemsByRarity;
using GameTools.Client.Application.UseCases.Items.GetItemsPage;
using GameTools.Client.Application.UseCases.Items.UpdateItem;
using GameTools.Client.Domain.Items;

namespace GameTools.Client.Console
{
    public class ItemTest(
        GetItemByIdUseCase getItemById,
        GetItemByRarityUseCase getItemByRarity,
        GetItemsPageUseCase getItemsPage,
        CreateItemUseCase createItem,
        DeleteItemUseCase deleteItem,
        UpdateItemUseCase updateItem,
        BulkInsertItemsUseCase bulkInsertItem,
        BulkUpdateItemsUseCase bulkUpdateItem)
    {
        public async Task Run()
        {
            //아이템 추가
            Item createdItem = await createItem.Handle(
                new CreateItemInput($"TestItem_{Stopwatch.GetTimestamp()}", 100, 1, null), CancellationToken.None);

            Item? getItem = await getItemById.Handle(createdItem.Id, CancellationToken.None);

            Item updatedItem = await updateItem.Handle(
                new UpdateItemInput(
                    createdItem.Id, createdItem.Name + "_updated", createdItem.Price, createdItem.Description,
                    createdItem.RarityId, createdItem.RowVersionBase64), CancellationToken.None);


            IReadOnlyList<Item> getByRarityItem = await getItemByRarity.Handle(createdItem.RarityId, CancellationToken.None);

            var getPageItem = await getItemsPage.Handle(
                new GetItemsPageInput(new Pagination(), null), CancellationToken.None);

            // 실패
            try
            {
                await deleteItem.Handle(new DeleteItemInput(createdItem.Id, createdItem.RowVersionBase64), CancellationToken.None);
                throw new Exception("Error : Row Version이 다르지만 Update 성공.");
            }
            catch { }

            // 성공
            await deleteItem.Handle(new DeleteItemInput(updatedItem.Id, updatedItem.RowVersionBase64), CancellationToken.None);

            // Bulk Insert
            BulkInsertItemsOutput bulkInsertOutput = await bulkInsertItem.Handle(
                new BulkInsertItemsInput([
                    new BulkInsertItemInputRow($"Bulk1_{Stopwatch.GetTimestamp()}", 1000, "BulkInsertTest", 1),
                    new BulkInsertItemInputRow($"Bulk2_{Stopwatch.GetTimestamp()}", 1000, "BulkInsertTest", 1),
                    new BulkInsertItemInputRow($"Bulk3_{Stopwatch.GetTimestamp()}", 1000, "BulkInsertTest", 1),
                    new BulkInsertItemInputRow($"Bulk4_{Stopwatch.GetTimestamp()}", 1000, "BulkInsertTest", 1),
                ]), CancellationToken.None);

            // Get Bulk Names
            PagedOutput<Item> pagedOutput = await getItemsPage.Handle(new GetItemsPageInput(new Pagination(1, 100), new ItemSearchFilter("Bulk", null)), CancellationToken.None);

            // Bulk Update
            var bulkupdateItemInputRows = new List<BulkUpdateItemInputRow>();
            foreach (var output in pagedOutput.Items)
            {
                bulkupdateItemInputRows.Add(
                    new(
                        output.Id, output.Name + "_updated", output.Price + 10000,
                        output.Description + "bulk update test",
                        output.RarityId, output.RowVersionBase64));
            }
            BulkUpdateItemsOutput bulkUpdateOutput = await bulkUpdateItem.Handle(new BulkUpdateItemsInput(bulkupdateItemInputRows), CancellationToken.None);

            // Delete
            PagedOutput<Item> pagedOutput2 = await getItemsPage.Handle(new GetItemsPageInput(new Pagination(1, 100), new ItemSearchFilter("Bulk", null)), CancellationToken.None);
            foreach (var item in pagedOutput2.Items)
            {
                await deleteItem.Handle(new DeleteItemInput(item.Id, item.RowVersionBase64), CancellationToken.None);
            }

        }
    }
}
