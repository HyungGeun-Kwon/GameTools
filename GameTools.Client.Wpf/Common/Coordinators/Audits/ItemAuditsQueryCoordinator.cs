using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GameTools.Client.Application.UseCases.Audits.GetItemAuditsPage;
using GameTools.Client.Wpf.Common.Enums;
using GameTools.Client.Wpf.Common.State;
using GameTools.Client.Wpf.ViewModels.Items.Mappers;

namespace GameTools.Client.Wpf.Common.Coordinators.Audits
{
    public sealed partial class ItemAuditsQueryCoordinator : ObservableObject, IItemAuditsQueryCoordinator
    {
        private readonly IItemAuditPageSearchState _itemAuditPageSearchState;
        private readonly GetItemAuditsPageUseCase _getItemAuditPageUseCase;

        private CancellationTokenSource? _cts;
      
        public ItemAuditsQueryCoordinator(IItemAuditPageSearchState itemAuditPageSearchState, GetItemAuditsPageUseCase getItemAuditPageUseCase)
        {
            _itemAuditPageSearchState = itemAuditPageSearchState;
            _getItemAuditPageUseCase = getItemAuditPageUseCase;

            _itemAuditPageSearchState.BusyState.PropertyChanged += OnItemAuditPageSearchStatePropertyChanged;
        }

        private void OnItemAuditPageSearchStatePropertyChanged(object? _, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(_itemAuditPageSearchState.BusyState.QueryBusy)) CancelCommand.NotifyCanExecuteChanged();
        }

        private bool CanCancel() => _itemAuditPageSearchState.BusyState.QueryBusy;

        [RelayCommand(CanExecute = nameof(CanCancel))]
        private void Cancel() => _cts?.Cancel();

        private CancellationToken NewToken(CancellationToken external)
        {
            try { _cts?.Cancel(); } catch (ObjectDisposedException) { }
            _cts = CancellationTokenSource.CreateLinkedTokenSource(external);
            return _cts.Token;
        }

        /// <summary>
        /// 동일한 필터로 재검색
        /// </summary>
        public Task RefreshAsync(CancellationToken external = default)
            => UpdatePageSearchState(_itemAuditPageSearchState.ToGetItemAuditPageInput(), external);

        /// <summary>
        /// 지정한 페이지로 이동
        /// </summary>
        public Task GoToPageAsync(int page, CancellationToken external = default)
            => UpdatePageSearchState(_itemAuditPageSearchState.GetItemAuditPageInputFromNewPage(page), external);

        /// <summary>
        /// 새로운 필터로 검색
        /// </summary>
        public Task SearchWithFilterAsync(
            int page, int pageSize,
            int? itemId, AuditActionType? action, DateTime? fromUtc, DateTime? toUtc,
            CancellationToken external = default)
            => UpdatePageSearchState(new(
                new(page, pageSize), 
                new(itemId, action is null or AuditActionType.ALL ? null : action.Value.ToString(), fromUtc, toUtc)), external);

        private async Task UpdatePageSearchState(GetItemAuditsPageInput input, CancellationToken external)
        {
            if (input.Pagination.PageNumber == 0 || input.Pagination.PageSize == 0) { return; }

            var token = NewToken(external);
            var myCts = _cts;

            try
            {
                SetQueryBusy(true);
                var output = await _getItemAuditPageUseCase.Handle(input, token);
                // 성공적으로 Get 한 경우에 업데이트
                _itemAuditPageSearchState.ReplacePageResults(output);
                _itemAuditPageSearchState.ReplaceFilter(
                    input.Filter?.ItemId, input.Filter?.Action, input.Filter?.FromUtc, input.Filter?.ToUtc);
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
            _itemAuditPageSearchState.BusyState.QueryBusy = value;
            CancelCommand.NotifyCanExecuteChanged();
        }

        public void Dispose()
        {
            _itemAuditPageSearchState.BusyState.PropertyChanged -= OnItemAuditPageSearchStatePropertyChanged;
            try { _cts?.Cancel(); } catch { }
            _cts?.Dispose();
        }
    }
}
