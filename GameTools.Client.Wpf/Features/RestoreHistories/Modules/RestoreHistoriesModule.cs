using DotNetHelper.MsDiKit.Extensions;
using GameTools.Client.Wpf.Features.Rarities.Models;
using GameTools.Client.Wpf.Features.RestoreHistories.Coordinators;
using GameTools.Client.Wpf.Features.RestoreHistories.State;
using GameTools.Client.Wpf.Features.RestoreHistories.ViewModels;
using GameTools.Client.Wpf.Features.RestoreHistories.Views;
using GameTools.Client.Wpf.Shared.Names;
using GameTools.Client.Wpf.Shared.State;
using Microsoft.Extensions.DependencyInjection;

namespace GameTools.Client.Wpf.Features.RestoreHistories.Modules
{
    public static class RestoreHistoriesModule
    {
        public static IServiceCollection AddRestoreHistoriesFeature(this IServiceCollection services)
        {

            services.AddSingleton<IRestoreHistoriesQueryCoordinator, RestoreHistoriesQueryCoordinator>();
            services.AddSingleton<IRestoreHistoryPageSearchState, RestoreHistoryPageSearchState>();

            services.AddSingleton<ISearchState<RarityEditModel>, SearchState<RarityEditModel>>();

            services.AddRegionView<RestoreHistoryHostView, RestoreHistoryHostViewModel>(RegionViewNames.Restore_HostView);
            services.AddRegionView<RestoreHistoryHeaderView, RestoreHistoryHeaderViewModel>(RegionViewNames.Restore_HeaderView);
            services.AddRegionView<RestoreHistoryResultView, RestoreHistoryResultViewModel>(RegionViewNames.Restore_ResultView);
            services.AddRegionView<RestoreHistoryPagingView, RestoreHistoryPagingViewModel>(RegionViewNames.Restore_PagingView);

            return services;
        }
    }
}
