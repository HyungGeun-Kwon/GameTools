using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DotNetHelper.MsDiKit.Common;
using DotNetHelper.MsDiKit.DialogServices;
using DotNetHelper.MsDiKit.RegionServices;
using GameTools.Client.Application.UseCases.Rarities.GetAllRarities;
using GameTools.Client.Wpf.Common.Names;
using GameTools.Client.Wpf.Common.State;
using GameTools.Client.Wpf.ViewModels.Rarities.Contracts;
using GameTools.Client.Wpf.ViewModels.Rarities.Mappers;

namespace GameTools.Client.Wpf.ViewModels.Rarities
{
    public sealed partial class RarityHeaderViewModel(
        IDialogService dialogService,
        ISearchState<RarityEditModel> raritySearchState,
        GetAllRaritiesUseCase getAllRaritiesUseCase)
        : ObservableObject, IRegionViewModel
    {
        [RelayCommand(IncludeCancelCommand = true, AllowConcurrentExecutions = false)]
        public async Task RaritySearch(CancellationToken token)
        {
            var result = await getAllRaritiesUseCase.Handle(token);
            raritySearchState.ReplaceResults(result.ToEditModels());
        }

        [RelayCommand]
        public void AddRarity()
        {
            dialogService.ShowDialog(DialogViewNames.Rarity_EditDialog, null, RarityEditDialogClosed);
        }

        private async void RarityEditDialogClosed(IDialogResult? result)
        {
            if (result?.ButtonResult != ButtonResult.OK) return;

            try
            {
                if (RaritySearchCommand.IsRunning)
                    RaritySearchCancelCommand.Execute(null);

                await RaritySearchCommand.ExecuteAsync(null); // 추가했다면 업데이트
            }
            catch (OperationCanceledException) { }
        }

        public void OnRegionActivated(Parameters? _) { }
        public void OnRegionDeactivated() { }
    }
}