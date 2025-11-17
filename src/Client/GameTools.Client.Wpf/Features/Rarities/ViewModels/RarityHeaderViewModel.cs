using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DotNetHelper.MsDiKit.Common;
using DotNetHelper.MsDiKit.DialogServices;
using DotNetHelper.MsDiKit.RegionServices;
using GameTools.Client.Wpf.Features.Rarities.Coordinators;
using GameTools.Client.Wpf.Shared.Names;

namespace GameTools.Client.Wpf.Features.Rarities.ViewModels
{
    public sealed partial class RarityHeaderViewModel(
        IDialogService dialogService,
        IRaritiesQueryCoordinator raritiesQueryCoordinator
        ) : ObservableObject, IRegionViewModel
    {
        [RelayCommand(AllowConcurrentExecutions = false)]
        private Task RaritySearchAsync()
            => raritiesQueryCoordinator.SearchAllAsync();

        [RelayCommand]
        private void AddRarity()
        {
            dialogService.ShowDialog(DialogViewNames.Rarity_EditDialog, null, RarityEditDialogClosed);
        }

        private async void RarityEditDialogClosed(IDialogResult? result)
        {
            if (result?.ButtonResult != ButtonResult.OK) return;

            await raritiesQueryCoordinator.SearchAllAsync();
        }

        public void OnRegionActivated(Parameters? _) { }
        public void OnRegionDeactivated() { }
    }
}