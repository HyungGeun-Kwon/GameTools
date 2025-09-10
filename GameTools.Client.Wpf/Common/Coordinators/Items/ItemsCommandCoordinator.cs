using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GameTools.Client.Application.UseCases.Items.BulkDeleteItems;
using GameTools.Client.Application.UseCases.Items.BulkInsertItems;
using GameTools.Client.Application.UseCases.Items.BulkUpdateItems;
using GameTools.Client.Application.UseCases.Items.CreateItem;
using GameTools.Client.Application.UseCases.Items.DeleteItem;
using GameTools.Client.Application.UseCases.Items.GetItemsPage;
using GameTools.Client.Application.UseCases.Items.UpdateItem;
using GameTools.Client.Domain.Items;
using GameTools.Client.Wpf.Common.State;
using GameTools.Client.Wpf.Models.Items;
using GameTools.Client.Wpf.ViewModels.Items.Contracts;
using GameTools.Client.Wpf.ViewModels.Items.Mappers;

namespace GameTools.Client.Wpf.Common.Coordinators.Items
{
    public sealed partial class ItemsCommandCoordinator : ObservableObject, IItemsCommandCoordinator
    {
        private readonly IItemPageSearchState _itemPageSearchState;
        private readonly UpdateItemUseCase _updateItemUseCase;
        private readonly DeleteItemUseCase _deleteItemUseCase;
        private readonly CreateItemUseCase _createItemUseCase;
        private readonly BulkInsertItemsUseCase _bulkInsertItemsUseCase;
        private readonly BulkUpdateItemsUseCase _bulkUpdateItemsUseCase;
        private readonly BulkDeleteItemsUseCase _bulkDeleteItemsUseCase;

        private CancellationTokenSource? _cts;

        public ItemsCommandCoordinator(
            IItemPageSearchState itemPageSearchState,
            GetItemsPageUseCase getItemPageUseCase,
            UpdateItemUseCase updateItemUseCase,
            DeleteItemUseCase deleteItemUseCase,
            CreateItemUseCase createItemUseCase,
            BulkInsertItemsUseCase bulkInsertItemsUseCase,
            BulkUpdateItemsUseCase bulkUpdateItemsUseCase,
            BulkDeleteItemsUseCase bulkDeleteItemsUseCase)
        {
            _itemPageSearchState = itemPageSearchState;
            _updateItemUseCase = updateItemUseCase;
            _deleteItemUseCase = deleteItemUseCase;
            _createItemUseCase = createItemUseCase;
            _bulkInsertItemsUseCase = bulkInsertItemsUseCase;
            _bulkUpdateItemsUseCase = bulkUpdateItemsUseCase;
            _bulkDeleteItemsUseCase = bulkDeleteItemsUseCase;

            _itemPageSearchState.BusyState.PropertyChanged += OnItemPageSearchStatePropertyChanged;
        }

        private void OnItemPageSearchStatePropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(_itemPageSearchState.BusyState.CommandBusy)) CancelCommand.NotifyCanExecuteChanged();
        }

        private bool CanCancel() => _itemPageSearchState.BusyState.CommandBusy;

        [RelayCommand(CanExecute = nameof(CanCancel))]
        private void Cancel() => _cts?.Cancel();

        private CancellationToken NewToken(CancellationToken external)
        {
            try { _cts?.Cancel(); } catch (ObjectDisposedException) { }
            _cts = CancellationTokenSource.CreateLinkedTokenSource(external);
            return _cts.Token;
        }

        public Task<Item> CreateAsync(ItemCreateModel itemCreateModel, CancellationToken external = default)
            => RunExclusiveCommandAsync(ct => _createItemUseCase.Handle(itemCreateModel.ToCreateItemInput(), ct), external);

        public Task DeleteAsync(ItemEditModel itemEditModel, CancellationToken external = default)
            => RunExclusiveCommandAsync(ct => _deleteItemUseCase.Handle(itemEditModel.ToDeleteItemInput(), ct), external);

        public Task<Item> UpdateAsync(ItemEditModel itemEditModel, CancellationToken external = default)
            => RunExclusiveCommandAsync(ct => _updateItemUseCase.Handle(itemEditModel.ToUpdateItemInput(), ct), external);

        public Task<BulkInsertItemsOutput> BulkInsertAsync(BulkInsertItemsInput bulkInsertItemsInput, CancellationToken external = default)
            => RunExclusiveCommandAsync(ct => _bulkInsertItemsUseCase.Handle(bulkInsertItemsInput, ct), external);

        public Task<BulkUpdateItemsOutput> BulkUpdateAsync(BulkUpdateItemsInput bulkUpdateItemsInput, CancellationToken external = default)
            => RunExclusiveCommandAsync(ct => _bulkUpdateItemsUseCase.Handle(bulkUpdateItemsInput, ct), external);

        public Task<BulkDeleteItemsOutput> BulkDeleteAsync(BulkDeleteItemsInput bulkDeleteItemsInput, CancellationToken external = default)
            => RunExclusiveCommandAsync(ct => _bulkDeleteItemsUseCase.Handle(bulkDeleteItemsInput, ct), external);

        private async Task RunExclusiveCommandAsync(
            Func<CancellationToken, Task> action,
            CancellationToken external = default)
        {
            var token = NewToken(external);
            var myCts = _cts;

            try
            {
                SetCommandBusy(true);
                await action(token);
            }
            finally
            {
                if (ReferenceEquals(myCts, _cts))
                    SetCommandBusy(false);
                myCts?.Dispose();
            }
        }

        private async Task<TResult> RunExclusiveCommandAsync<TResult>(
            Func<CancellationToken, Task<TResult>> action,
            CancellationToken external = default)
        {
            var token = NewToken(external);
            var myCts = _cts;

            try
            {
                SetCommandBusy(true);
                return await action(token);
            }
            finally
            {
                if (ReferenceEquals(myCts, _cts))
                {
                    SetCommandBusy(false);
                    _cts = null;
                }
                myCts?.Dispose();
            }
        }

        private void SetCommandBusy(bool value)
        {
            _itemPageSearchState.BusyState.CommandBusy = value;
            CancelCommand.NotifyCanExecuteChanged();
        }

        public void Dispose()
        {
            _itemPageSearchState.BusyState.PropertyChanged -= OnItemPageSearchStatePropertyChanged;
            try { _cts?.Cancel(); } catch { }
            _cts?.Dispose();
        }
    }
}
