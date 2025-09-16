using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DotNetHelper.MsDiKit.Common;
using DotNetHelper.MsDiKit.DialogServices;
using GameTools.Client.Wpf.Common.Coordinators.Rarities;
using GameTools.Client.Wpf.Models.Rarities;
using Serilog;

namespace GameTools.Client.Wpf.ViewModels.Rarities
{
    public partial class RarityCreateViewModel(IRaritiesCommandCoordinator raritiesCommandCoordinator) : ObservableObject, IDialogViewModel
    {
        public RarityCreateModel RarityCreateModel { get; } = new();

        public event Action<IDialogResult>? RequestClose;

        [RelayCommand(IncludeCancelCommand = true, AllowConcurrentExecutions = false)]
        private async Task SaveRarityAsync(CancellationToken ct)
        {
            if (RarityCreateModel.HasErrors)
            {
                MessageBox.Show("Please check the input values.", "Warning");
                return;
            }

            try
            {
                await raritiesCommandCoordinator.CreateAsync(RarityCreateModel, ct);
                RequestClose?.Invoke(new DialogResult(ButtonResult.OK));
            }
            catch (OperationCanceledException) { }
            catch (Exception ex)
            {
                Log.Error(ex, "Rarity Add Fail.");
                MessageBox.Show($"Rarity Add Fail.\r\n {ex.Message}");
            }
        }

        [RelayCommand]
        private void CancelRarityCreate()
        {
            if (SaveRarityCommand.IsRunning) { SaveRarityCancelCommand.Execute(null); }
            RequestClose?.Invoke(new DialogResult(ButtonResult.Cancel));
        }

        public void OnDialogClosed() { }

        public void OnDialogOpened(Parameters? parameters) { }
    }
}
