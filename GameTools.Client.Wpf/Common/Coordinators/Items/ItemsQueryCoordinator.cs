using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GameTools.Client.Application.UseCases.Items.GetItemsPage;
using GameTools.Client.Wpf.Common.State;
using GameTools.Client.Wpf.ViewModels.Items.Mappers;

namespace GameTools.Client.Wpf.Common.Coordinators.Items
{
    public sealed partial class ItemsQueryCoordinator : ObservableObject, IItemsQueryCoordinator
    {
        private readonly IItemPageSearchState _itemPageSearchState;
        private readonly GetItemsPageUseCase _getItemPageUseCase;

        private CancellationTokenSource? _cts;

        public ItemsQueryCoordinator(IItemPageSearchState itemPageSearchState, GetItemsPageUseCase getItemsPageUseCase)
        {
            _itemPageSearchState = itemPageSearchState;
            _getItemPageUseCase = getItemsPageUseCase;

            _itemPageSearchState.BusyState.PropertyChanged += OnItemPageSearchStatePropertyChanged;
        }

        private void OnItemPageSearchStatePropertyChanged(object? _, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(_itemPageSearchState.BusyState.QueryBusy)) CancelCommand.NotifyCanExecuteChanged();
        }

        private bool CanCancel() => _itemPageSearchState.BusyState.QueryBusy;

        [RelayCommand(CanExecute = nameof(CanCancel))]
        private void Cancel() => _cts?.Cancel();

        private CancellationToken NewToken(CancellationToken external)
        {
            try { _cts?.Cancel(); } catch (ObjectDisposedException) { }
            _cts = CancellationTokenSource.CreateLinkedTokenSource(external);
            return _cts.Token;
        }

        /// <summary>
        /// 동일한 필터로 재검색 (Delete, Create 이후 등)
        /// </summary>
        public Task RefreshAsync(CancellationToken external = default)
            => UpdatePageSearchState(_itemPageSearchState.ToGetItemPageInput(), external);

        /// <summary>
        /// 지정한 페이지로 이동
        /// </summary>
        public Task GoToPageAsync(int page, CancellationToken external = default)
            => UpdatePageSearchState(_itemPageSearchState.GetItemPageInputFromNewPage(page), external);

        /// <summary>
        /// 새로운 필터로 검색
        /// </summary>
        public Task SearchWithFilterAsync(
            int page, int pageSize,
            string? nameFilter, byte? rarityIdFilter,
            CancellationToken external = default)
            => UpdatePageSearchState(new(new(page, pageSize), new(nameFilter, rarityIdFilter)), external);

        private async Task UpdatePageSearchState(GetItemsPageInput input, CancellationToken external)
        {
            if (input.Pagination.PageNumber == 0 || input.Pagination.PageSize == 0) { return; }

            var token = NewToken(external);
            var myCts = _cts;

            try
            {
                SetQueryBusy(true);
                var output = await _getItemPageUseCase.Handle(input, token);
                // 성공적으로 Get 한 경우에 업데이트
                _itemPageSearchState.ReplacePageResults(output.ToPagedItemEditModel());
                _itemPageSearchState.ReplaceFilter(input.Filter?.NameSearch, input.Filter?.RarityId);
            }
            finally
            {
                if (ReferenceEquals(myCts, _cts))
                {
                    SetQueryBusy(false);
                    _cts = null;
                }
                myCts?.Dispose();
            }
        }

        private void SetQueryBusy(bool value)
        {
            _itemPageSearchState.BusyState.QueryBusy = value;
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
