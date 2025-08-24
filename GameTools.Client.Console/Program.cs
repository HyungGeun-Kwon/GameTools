using System.Diagnostics;
using GameTools.Client.Application.Common.Paging;
using GameTools.Client.Application.Ports;
using GameTools.Client.Application.UseCases.BulkInsertItems;
using GameTools.Client.Application.UseCases.CreateItem;
using GameTools.Client.Application.UseCases.DeleteItem;
using GameTools.Client.Application.UseCases.GetByRarity;
using GameTools.Client.Application.UseCases.GetItemById;
using GameTools.Client.Application.UseCases.GetItemsPage;
using GameTools.Client.Domain.Items;
using GameTools.Client.Infrastructure.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        var baseUrl = "http://localhost:5008";
        services.AddHttpClient<IItemsGateway, ItemsGateway>(c => c.BaseAddress = new Uri(baseUrl));
        services.AddTransient<GetItemByIdUseCase>();
        services.AddTransient<GetItemByRarityUseCase>();
        services.AddTransient<GetItemsPageUseCase>();
        services.AddTransient<CreateItemUseCase>();
        services.AddTransient<DeleteItemUseCase>();
        services.AddTransient<UpdateItemUseCase>();
        services.AddTransient<BulkInsertItemsUseCase>();
    }).Build();


var scope = host.Services.CreateScope();

var getItemById = scope.ServiceProvider.GetRequiredService<GetItemByIdUseCase>();
var getItemByRarity = scope.ServiceProvider.GetRequiredService<GetItemByRarityUseCase>();
var getItemsPage = scope.ServiceProvider.GetRequiredService<GetItemsPageUseCase>();
var createItem = scope.ServiceProvider.GetRequiredService<CreateItemUseCase>();
var deleteItem = scope.ServiceProvider.GetRequiredService<DeleteItemUseCase>();
var updateItem = scope.ServiceProvider.GetRequiredService<UpdateItemUseCase>();
var bulkInsertItem = scope.ServiceProvider.GetRequiredService<BulkInsertItemsUseCase>();

try
{
    //아이템 추가
    Item createdItem = await createItem.Handle(
        new CreateItemInput($"TestItem_{Stopwatch.GetTimestamp()}", 100, 1, null), CancellationToken.None);

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
            new BulkInsertItemInputRow($"Bulk1_{Stopwatch.GetTimestamp()}", 1000, 1, null),
            new BulkInsertItemInputRow($"Bulk2_{Stopwatch.GetTimestamp()}", 1000, 1, null),
            new BulkInsertItemInputRow($"Bulk3_{Stopwatch.GetTimestamp()}", 1000, 1, null),
            new BulkInsertItemInputRow($"Bulk4_{Stopwatch.GetTimestamp()}", 1000, 1, null),
        ]), CancellationToken.None);

    // Get Bulk Names
    PagedOutput<Item> pagedOutput = await getItemsPage.Handle(new GetItemsPageInput(new Pagination(1, 100), new ItemSearchFilter("Bulk", null)), CancellationToken.None);

    // TODO : bulk update 예정

    foreach (var item in pagedOutput.Items)
    {
        await deleteItem.Handle(new DeleteItemInput(item.Id, item.RowVersionBase64), CancellationToken.None);
    }
}
catch (HttpRequestException ex)
{
    Console.WriteLine($"HTTP Error : {ex.Message}");
}
catch (Exception ex)
{
    Console.WriteLine($"Error: {ex.Message}");
}
