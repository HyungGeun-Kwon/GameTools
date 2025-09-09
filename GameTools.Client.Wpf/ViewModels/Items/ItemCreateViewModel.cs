using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DotNetHelper.MsDiKit.Common;
using DotNetHelper.MsDiKit.DialogServices;
using GameTools.Client.Wpf.Common.Coordinators.Items;
using GameTools.Client.Wpf.Models.Items;
using Serilog;

namespace GameTools.Client.Wpf.ViewModels.Items
{
    public partial class ItemCreateViewModel(
        IItemsCommandCoordinator itemsCommandCoordinator,
        RarityLookupViewModel rarityLookupViewModel
        ) : ObservableObject, IDialogViewModel
    {
        public RarityLookupViewModel RarityLookup => rarityLookupViewModel;

        public ItemCreateModel ItemCreateModel { get; } = new();

        public event Action<IDialogResult>? RequestClose;

        [RelayCommand(IncludeCancelCommand = true, AllowConcurrentExecutions = false)]
        private async Task SaveItemAsync(CancellationToken ct)
        {
            if (ItemCreateModel.HasErrors)
            {
                MessageBox.Show("Please check the input values.", "Warning");
                return;
            }

            try
            {
                await itemsCommandCoordinator.CreateAsync(ItemCreateModel, ct);
                RequestClose?.Invoke(new DialogResult(ButtonResult.OK));
            }
            catch (OperationCanceledException) { }
            catch (Exception ex)
            {
                Log.Error(ex, "Item Add Fail.");
                MessageBox.Show($"Item Add Fail.\r\n {ex.Message}");
            }
        }

        [RelayCommand]
        private void CancelItemCreate()
        {
            if (SaveItemCommand.IsRunning) { SaveItemCancelCommand.Execute(null); }
            RequestClose?.Invoke(new DialogResult(ButtonResult.Cancel));
        }

        public void OnDialogClosed() => RarityLookup.LoadCancelCommand.Execute(null);
        public async void OnDialogOpened(Parameters? parameters)
            => await RarityLookup.LoadCommand.ExecuteAsync(null);
    }
}
