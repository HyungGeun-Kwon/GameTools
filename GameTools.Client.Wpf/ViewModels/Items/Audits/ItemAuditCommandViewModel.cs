using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DotNetHelper.MsDiKit.Common;
using DotNetHelper.MsDiKit.DialogServices;
using DotNetHelper.MsDiKit.RegionServices;
using GameTools.Client.Wpf.Common.Names;

namespace GameTools.Client.Wpf.ViewModels.Items.Audits
{
    public sealed partial class ItemAuditCommandViewModel(IDialogService dialogService) : ObservableObject, IRegionViewModel
    {
        [RelayCommand]
        public void RestoreAsOf()
            => dialogService.ShowDialog(DialogViewNames.Item_RestoreAsOfDialog, null, ItemRestoreAsOfDialogClosed);

        private void ItemRestoreAsOfDialogClosed(IDialogResult? result)
        {
            if (result?.ButtonResult != ButtonResult.OK) return;

            try
            {
                
            }
            catch (OperationCanceledException) { }
        }

        public void OnRegionActivated(Parameters? parameters)
        {
        }
        public void OnRegionDeactivated()
        {
        }
    }
}
