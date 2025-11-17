using DotNetHelper.MsDiKit.Extensions;
using GameTools.Client.Wpf.Features.Rarities.Coordinators;
using GameTools.Client.Wpf.Features.Rarities.Models;
using GameTools.Client.Wpf.Features.Rarities.ViewModels;
using GameTools.Client.Wpf.Features.Rarities.Views;
using GameTools.Client.Wpf.Shared.Components.Lookups.Rarities;
using GameTools.Client.Wpf.Shared.Names;
using GameTools.Client.Wpf.Shared.State;
using Microsoft.Extensions.DependencyInjection;

namespace GameTools.Client.Wpf.Features.Rarities.Modules
{
    public static class RarityModule
    {
        public static IServiceCollection AddRaritiesFeature(this IServiceCollection services)
        {
            services.AddSingleton<ISearchState<RarityEditModel>, SearchState<RarityEditModel>>();

            services
                .AddSingleton<IRaritiesQueryCoordinator, RaritiesQueryCoordinator>()
                .AddSingleton<IRaritiesCommandCoordinator, RaritiesCommandCoordinator>();

            services
                .AddRegionView<RarityHostView, RarityHostViewModel>(RegionViewNames.Rarity_HostView)
                .AddRegionView<RarityHeaderView, RarityHeaderViewModel>(RegionViewNames.Rarity_HeaderView)
                .AddRegionView<RarityResultView, RarityResultViewModel>(RegionViewNames.Rarity_ResultView)
                .AddDialogView<RarityCreateView, RarityCreateViewModel>(DialogViewNames.Rarity_EditDialog);

            services.AddTransient<RarityLookupViewModel>();

            return services;
        }
    }
}
