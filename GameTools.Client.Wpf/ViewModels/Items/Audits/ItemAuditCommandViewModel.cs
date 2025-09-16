using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DotNetHelper.MsDiKit.Common;
using DotNetHelper.MsDiKit.DialogServices;
using DotNetHelper.MsDiKit.RegionServices;
using GameTools.Client.Wpf.Common.Coordinators.Audits;
using GameTools.Client.Wpf.Common.Names;
using GameTools.Client.Wpf.Common.State;

namespace GameTools.Client.Wpf.ViewModels.Items.Audits
{
    public sealed partial class ItemAuditCommandViewModel(
        IDialogService dialogService,
        IItemAuditPageSearchState itemAuditPageSearchState,
        IItemAuditsQueryCoordinator itemAuditsQueryCoordinator) : ObservableObject, IRegionViewModel
    {
        [RelayCommand]
        public void RestoreAsOf()
            => dialogService.ShowDialog(DialogViewNames.Item_RestoreAsOfDialog, null, ItemRestoreAsOfDialogClosed);

        private async void ItemRestoreAsOfDialogClosed(IDialogResult? result)
        {
            if (result?.ButtonResult != ButtonResult.OK) return;

            try
            {
                if (itemAuditPageSearchState.Results.Count == 0) return;

                await itemAuditsQueryCoordinator.RefreshAsync();
            }
            catch (OperationCanceledException) { }
        }

        public void OnRegionActivated(Parameters? _) { }
        public void OnRegionDeactivated() { }
    }
}
