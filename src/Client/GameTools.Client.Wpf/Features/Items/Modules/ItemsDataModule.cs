using DotNetHelper.MsDiKit.Extensions;
using GameTools.Client.Wpf.Features.Items.Data.Coordinators;
using GameTools.Client.Wpf.Features.Items.Data.State;
using GameTools.Client.Wpf.Features.Items.Data.ViewModels;
using GameTools.Client.Wpf.Features.Items.Data.Views;
using GameTools.Client.Wpf.Shared.Names;
using Microsoft.Extensions.DependencyInjection;

namespace GameTools.Client.Wpf.Features.Items.Modules
{
    public static class ItemsDataModule
    {
        public static IServiceCollection AddItemsDataFeature(this IServiceCollection services)
        {
            services.AddSingleton<IItemPageSearchState, ItemPageSearchState>();

            services
                .AddSingleton<IItemsQueryCoordinator, ItemsQueryCoordinator>()
                .AddSingleton<IItemsCommandCoordinator, ItemsCommandCoordinator>()
                .AddSingleton<IItemsCsvCommandCoordinator, ItemsCsvCommandCoordinator>();

            services.AddRegionView<ItemDataHostView, ItemDataHostViewModel>(RegionViewNames.Item_Data_HostView)
                .AddRegionView<ItemDataHeaderView, ItemDataHeaderViewModel>(RegionViewNames.Item_Data_HeaderView)
                .AddRegionView<ItemDataCommandView, ItemDataCommandViewModel>(RegionViewNames.Item_Data_CommandView)
                .AddRegionView<ItemDataSearchView, ItemDataSearchViewModel>(RegionViewNames.Item_Data_SearchView)
                .AddRegionView<ItemDataResultView, ItemDataResultViewModel>(RegionViewNames.Item_Data_ResultView)
                .AddRegionView<ItemDataPagingView, ItemDataPagingViewModel>(RegionViewNames.Item_Data_PagingView)
                .AddDialogView<ItemDataCreateView, ItemDataCreateViewModel>(DialogViewNames.Item_EditDialog);

            return services;
        }
    }
}
