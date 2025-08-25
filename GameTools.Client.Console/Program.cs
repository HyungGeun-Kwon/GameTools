using GameTools.Client.Application.Ports;
using GameTools.Client.Application.UseCases.Items.BulkInsertItems;
using GameTools.Client.Application.UseCases.Items.BulkUpdateItems;
using GameTools.Client.Application.UseCases.Items.CreateItem;
using GameTools.Client.Application.UseCases.Items.DeleteItem;
using GameTools.Client.Application.UseCases.Items.GetItemById;
using GameTools.Client.Application.UseCases.Items.GetItemsByRarity;
using GameTools.Client.Application.UseCases.Items.GetItemsPage;
using GameTools.Client.Application.UseCases.Items.UpdateItem;
using GameTools.Client.Console;
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
        services.AddTransient<BulkUpdateItemsUseCase>();
        services.AddTransient<ItemTest>();
        
        services.AddTransient<RarityTest>();
    }).Build();


var scope = host.Services.CreateScope();
var itemTest = scope.ServiceProvider.GetRequiredService<ItemTest>();
var rarityTest = scope.ServiceProvider.GetRequiredService<RarityTest>();

try
{
    //await itemTest.Run();
    //await rarityTest.Run();
}
catch (HttpRequestException ex)
{
    Console.WriteLine($"HTTP Error : {ex.Message}");
}
catch (Exception ex)
{
    Console.WriteLine($"Error: {ex.Message}");
}
