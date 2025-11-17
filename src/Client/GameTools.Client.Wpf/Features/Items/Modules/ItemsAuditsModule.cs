using DotNetHelper.MsDiKit.Extensions;
using GameTools.Client.Wpf.Features.Items.Audits.Coordinators;
using GameTools.Client.Wpf.Features.Items.Audits.State;
using GameTools.Client.Wpf.Features.Items.Audits.ViewModels;
using GameTools.Client.Wpf.Features.Items.Audits.Views;
using GameTools.Client.Wpf.Shared.Names;
using Microsoft.Extensions.DependencyInjection;

namespace GameTools.Client.Wpf.Features.Items.Modules
{
    public static class ItemsAuditsModule
    {
        public static IServiceCollection AddItemsAuditsFeature(this IServiceCollection services)
        {
            services.AddSingleton<IItemAuditPageSearchState, ItemAuditPageSearchState>();

            services.AddSingleton<IItemAuditsQueryCoordinator, ItemAuditsQueryCoordinator>();

            services
                .AddRegionView<ItemAuditHostView, ItemAuditHostViewModel>(RegionViewNames.Item_Audit_HostView)
                .AddRegionView<ItemAuditHeaderView, ItemAuditHeaderViewModel>(RegionViewNames.Item_Audit_HeaderView)
                .AddRegionView<ItemAuditCommandView, ItemAuditCommandViewModel>(RegionViewNames.Item_Audit_CommandView)
                .AddRegionView<ItemAuditSearchView, ItemAuditSearchViewModel>(RegionViewNames.Item_Audit_SearchView)
                .AddRegionView<ItemAuditResultView, ItemAuditResultViewModel>(RegionViewNames.Item_Audit_ResultView)
                .AddRegionView<ItemAuditPagingView, ItemAuditPagingViewModel>(RegionViewNames.Item_Audit_PagingView)
                .AddDialogView<ItemAuditRestoreAsOfView, ItemAuditRestoreAsOfViewModel>(DialogViewNames.Item_RestoreAsOfDialog);

            return services;
        }
    }
}
