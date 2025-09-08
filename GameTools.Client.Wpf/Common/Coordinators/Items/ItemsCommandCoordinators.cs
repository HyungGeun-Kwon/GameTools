using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GameTools.Client.Application.UseCases.Items.BulkInsertItems;
using GameTools.Client.Application.UseCases.Items.CreateItem;
using GameTools.Client.Application.UseCases.Items.DeleteItem;
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

        private CancellationTokenSource? _cts;

        public ItemsCommandCoordinator(
            IItemPageSearchState itemPageSearchState,
            UpdateItemUseCase updateItemUseCase,
            DeleteItemUseCase deleteItemUseCase,
            CreateItemUseCase createItemUseCase,
            BulkInsertItemsUseCase bulkInsertItemsUseCase)
        {
            _itemPageSearchState = itemPageSearchState;
            _updateItemUseCase = updateItemUseCase;
            _deleteItemUseCase = deleteItemUseCase;
            _createItemUseCase = createItemUseCase;
            _bulkInsertItemsUseCase = bulkInsertItemsUseCase;

            _itemPageSearchState.BusyState.PropertyChanged += OnItemPageSearchStatePropertyChanged;
        }

        private void OnItemPageSearchStatePropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "CommandBusy") CancelCommand.NotifyCanExecuteChanged();
        }

        private bool CanCancel() => _itemPageSearchState.BusyState.CommandBusy;

        [RelayCommand(CanExecute = nameof(CanCancel))]
        private void Cancel() => _cts?.Cancel();

        private CancellationToken NewToken(CancellationToken external)
        {
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = CancellationTokenSource.CreateLinkedTokenSource(external);
            return _cts.Token;
        }

        public Task<Item> CreateAsync(ItemCreateModel itemCreateModel, bool throwCancelException = false, CancellationToken external = default)
            => RunExclusiveCommandAsync(ct => _createItemUseCase.Handle(itemCreateModel.ToCreateItemInput(), ct), throwCancelException, external);


        public Task DeleteAsync(ItemEditModel itemEditModel, bool throwCancelException = false, CancellationToken external = default)
            => RunExclusiveCommandAsync(ct => _deleteItemUseCase.Handle(itemEditModel.ToDeleteItemInput(), ct), throwCancelException, external);

        public Task<Item> UpdateAsync(ItemEditModel itemEditModel, bool throwCancelException = false, CancellationToken external = default)
            => RunExclusiveCommandAsync(ct => _updateItemUseCase.Handle(itemEditModel.ToUpdateItemInput(), ct), throwCancelException, external);

        //public Task<BulkInsertItemsOutput> BulkInsertAsync()
        //    => _bulkInsertItemsUseCase.Handle(new )

        private async Task RunExclusiveCommandAsync(
            Func<CancellationToken, Task> action,
            bool throwCancelException = false,
            CancellationToken external = default)
        {
            var token = NewToken(external);
            var myCts = _cts;

            try
            {
                SetCommandBusy(true);
                await action(token);
            }
            catch (OperationCanceledException)
            {
                if (throwCancelException) throw;
            }
            finally
            {
                if (ReferenceEquals(myCts, _cts))
                    SetCommandBusy(false);
            }
        }

        private async Task<TResult> RunExclusiveCommandAsync<TResult>(
            Func<CancellationToken, Task<TResult>> action,
            bool throwCancelException = false,
            CancellationToken external = default)
        {
            var token = NewToken(external);
            var myCts = _cts;

            try
            {
                SetCommandBusy(true);
                return await action(token);
            }
            catch (OperationCanceledException)
            {
                if (throwCancelException) throw;
                return default!;
            }
            finally
            {
                if (ReferenceEquals(myCts, _cts))
                    SetCommandBusy(false);
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
            _cts?.Cancel();
            _cts?.Dispose();
        }
    }
}
