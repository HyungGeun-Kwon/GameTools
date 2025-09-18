using DotNetHelper.MsDiKit.Extensions;
using GameTools.Client.Wpf.Features.Navigations.ViewModels;
using GameTools.Client.Wpf.Features.Navigations.Views;
using GameTools.Client.Wpf.Shared.Names;
using Microsoft.Extensions.DependencyInjection;

namespace GameTools.Client.Wpf.Features.Navigations.Modules
{
    public static class NavigationsModule
    {
        public static IServiceCollection AddNavigationsFeature(this IServiceCollection services)
        {
            services.AddRegionView<EntityNavigationView, EntityNavigationViewModel>(RegionViewNames.Main_EntityNavigationView);

            return services;
        }
    }
}
