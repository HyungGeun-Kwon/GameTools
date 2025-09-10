using GameTools.Client.Application.UseCases.Items.BulkDeleteItems;
using GameTools.Client.Application.UseCases.Items.BulkInsertItems;
using GameTools.Client.Application.UseCases.Items.BulkUpdateItems;
using GameTools.Client.Application.UseCases.Items.CreateItem;
using GameTools.Client.Application.UseCases.Items.DeleteItem;
using GameTools.Client.Application.UseCases.Items.GetItemById;
using GameTools.Client.Application.UseCases.Items.GetItemsByRarity;
using GameTools.Client.Application.UseCases.Items.GetItemsPage;
using GameTools.Client.Application.UseCases.Items.UpdateItem;
using GameTools.Client.Application.UseCases.Rarities.CreateRarity;
using GameTools.Client.Application.UseCases.Rarities.DeleteRarity;
using GameTools.Client.Application.UseCases.Rarities.GetAllRarities;
using GameTools.Client.Application.UseCases.Rarities.GetRarityById;
using GameTools.Client.Application.UseCases.Rarities.UpdateRarity;
using Microsoft.Extensions.DependencyInjection;

namespace GameTools.Client.Application.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddClientApplication(this IServiceCollection s)
        {
            // Items
            s.AddTransient<GetItemByIdUseCase>();
            s.AddTransient<GetItemByRarityUseCase>();
            s.AddTransient<GetItemsPageUseCase>();
            s.AddTransient<CreateItemUseCase>();
            s.AddTransient<DeleteItemUseCase>();
            s.AddTransient<UpdateItemUseCase>();
            s.AddTransient<BulkInsertItemsUseCase>();
            s.AddTransient<BulkUpdateItemsUseCase>();
            s.AddTransient<BulkDeleteItemsUseCase>();

            // Rarities
            s.AddTransient<GetRarityByIdUseCase>();
            s.AddTransient<GetAllRaritiesUseCase>();
            s.AddTransient<CreateRarityUseCase>();
            s.AddTransient<UpdateRarityUseCase>();
            s.AddTransient<DeleteRarityUseCase>();

            return s;
        }
    }
}
