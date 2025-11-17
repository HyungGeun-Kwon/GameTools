using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using DotNetHelper.MsDiKit.Common;
using DotNetHelper.MsDiKit.RegionServices;
using GameTools.Client.Application.Models.Restores;
using GameTools.Client.Domain.Items;
using GameTools.Client.Domain.Rarities;
using GameTools.Client.Wpf.Shared.Names;

namespace GameTools.Client.Wpf.Features.Navigations.ViewModels
{
    public sealed partial class EntityNavigationViewModel(IRegionService regionService) : ObservableObject, IRegionViewModel
    {
        public ObservableCollection<EntityNavigationItem> Entities { get; } =
        [
            new(nameof(Item), RegionViewNames.Item_HostView),
            new(nameof(Rarity), RegionViewNames.Rarity_HostView),
            new(nameof(RestoreHistory), RegionViewNames.Restore_HostView),
        ];

        [ObservableProperty]
        public partial EntityNavigationItem? SelectedEntity { get; set; }

        partial void OnSelectedEntityChanged(EntityNavigationItem? value)
        {
            if (value is null) return;

            regionService.SetView(RegionNames.Main_MainContentRegion, value.ViewName, value.Parameters);
        }

        public void OnRegionActivated(Parameters? _)
        {
            SelectedEntity = Entities.FirstOrDefault();
        }

        public void OnRegionDeactivated() { }
    }
}
