using DotNetHelper.MsDiKit.Extensions;
using GameTools.Client.Wpf.Features.Items.Host.ViewModels;
using GameTools.Client.Wpf.Features.Items.Host.Views;
using GameTools.Client.Wpf.Shared.Names;
using Microsoft.Extensions.DependencyInjection;

namespace GameTools.Client.Wpf.Features.Items.Modules
{
    public static class ItemsHostModule
    {
        public static IServiceCollection AddItemsHostFeature(this IServiceCollection services)
        {
            services.AddRegionView<ItemHostView, ItemHostViewModel>(RegionViewNames.Item_HostView);
            return services;
        }
    }
}
