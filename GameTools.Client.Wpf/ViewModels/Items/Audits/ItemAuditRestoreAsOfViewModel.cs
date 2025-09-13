using System.ComponentModel.DataAnnotations;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DotNetHelper.MsDiKit.Common;
using DotNetHelper.MsDiKit.DialogServices;
using GameTools.Client.Application.UseCases.Items.RestoreItemsAsOf;
using GameTools.Client.Wpf.Common.Coordinators.Items;
using GameTools.Client.Wpf.Common.State;
using Serilog;

namespace GameTools.Client.Wpf.ViewModels.Items.Audits
{
    public sealed partial class ItemAuditRestoreAsOfViewModel(
        IItemsCommandCoordinator itemsCommandCoordinator,
        IItemPageSearchState itemPageSearchState
        ) : ObservableValidator, IDialogViewModel
    {
        public IItemPageSearchState ItemPageSearchState => itemPageSearchState;

        [ObservableProperty]
        private DateTime _asOfUtc;

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Range(1, int.MaxValue)]
        private int? _itemId;

        [ObservableProperty]
        private bool _dryRun = false;

        [ObservableProperty]
        private string? _notes;

        public event Action<IDialogResult>? RequestClose;

        [RelayCommand(IncludeCancelCommand = true, AllowConcurrentExecutions = false)]
        private async Task RestoreAsOfItemAsync(CancellationToken ct)
        {
            if (HasErrors)
            {
                MessageBox.Show("Please check the input values.", "Warning");
                return;
            }

            try
            {
                await itemsCommandCoordinator.RestoreAsOfAsync(GetInput(), ct);
                RequestClose?.Invoke(new DialogResult(ButtonResult.OK));
            }
            catch (OperationCanceledException) { }
            catch(Exception ex)
            {
                Log.Error(
                    ex, 
                    $"Item Restore AsOf Fail. " + Environment.NewLine +
                    $"AsOf (Utc) : {AsOfUtc:yyyy-MM-dd HH:mm:ss}" + Environment.NewLine +
                    $"Item Id : {ItemId}" + Environment.NewLine +
                    $"Dry Run : {DryRun}" + Environment.NewLine +
                    $"Notes : {Notes}");

                MessageBox.Show($"Item Restore AsOf Fail.\r\n {ex.Message}");
            }
        }

        [RelayCommand]
        private void CancelRestoreAsOfItem()
        {
            if (RestoreAsOfItemCommand.IsRunning) { RestoreAsOfItemCancelCommand.Execute(null); }
            RequestClose?.Invoke(new DialogResult(ButtonResult.Cancel));
        }


        private RestoreItemsAsOfInput GetInput() => new(AsOfUtc, ItemId, DryRun, Notes);

        public void OnDialogClosed() { }

        public void OnDialogOpened(Parameters? parameters) => AsOfUtc = DateTime.UtcNow;
    }
}
