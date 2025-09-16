using GameTools.Client.Application.Extensions;
using GameTools.Client.Console;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using GameTools.Client.Infrastructure.Extensions;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        var baseUrl = "http://localhost:5008";

        services.AddClientInfrastructure<ConsoleTestActorProvider>(baseUrl);

        services.AddClientApplication();
        services.AddTransient<ItemTest>();
        services.AddTransient<RarityTest>();
    }).Build();


var scope = host.Services.CreateScope();
var itemTest = scope.ServiceProvider.GetRequiredService<ItemTest>();
var rarityTest = scope.ServiceProvider.GetRequiredService<RarityTest>();

try
{
    //await itemTest.Run();
    await rarityTest.Run();
}
catch (HttpRequestException ex)
{
    Console.WriteLine($"HTTP Error : {ex.Message}");
}
catch (Exception ex)
{
    Console.WriteLine($"Error: {ex.Message}");
}
