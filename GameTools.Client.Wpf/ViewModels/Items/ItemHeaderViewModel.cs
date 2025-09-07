using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DotNetHelper.MsDiKit.Common;
using DotNetHelper.MsDiKit.DialogServices;
using DotNetHelper.MsDiKit.RegionServices;
using GameTools.Client.Wpf.Common.Coordinators.Items;
using GameTools.Client.Wpf.Common.Names;
using GameTools.Client.Wpf.Common.State;

namespace GameTools.Client.Wpf.ViewModels.Items
{
    public sealed partial class ItemHeaderViewModel(
        IDialogService dialogService,
        IRegionService regionService,
        IItemPageSearchState itemPageSearchState,
        IItemsQueryCoordinator itemsQueryCoordinator
        ) : ObservableObject, IRegionViewModel
    {
        [RelayCommand]
        private void AddItem()
        {
            dialogService.ShowDialog(DialogViewNames.Item_EditDialog, null, ItemEditDialogClosed);
        }

        private async void ItemEditDialogClosed(IDialogResult? result)
        {
            if (result?.ButtonResult != ButtonResult.OK) return;

            try
            {
                if (itemPageSearchState.Results.Count == 0) return;

                // 가장 최근에 검색했던 방식으로 검색.
                await itemsQueryCoordinator.RefreshAsync();
            }
            catch (OperationCanceledException) { }
        }

        public void OnRegionActivated(Parameters? _)
        {
            regionService.SetView(RegionNames.Item_SearchRegion, RegionViewNames.Item_SearchView);
        }

        public void OnRegionDeactivated() { }
    }
}
