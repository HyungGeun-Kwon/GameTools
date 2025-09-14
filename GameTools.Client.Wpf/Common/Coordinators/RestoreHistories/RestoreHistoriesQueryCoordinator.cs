using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GameTools.Client.Application.UseCases.Restores.GetRestoreHistoryPage;
using GameTools.Client.Wpf.Common.State;
using GameTools.Client.Wpf.ViewModels.RestoreHistories.Mappers;

namespace GameTools.Client.Wpf.Common.Coordinators.RestoreHistories
{
    public sealed partial class RestoreHistoriesQueryCoordinator : ObservableObject, IRestoreHistoriesQueryCoordinator
    {
        private readonly IRestoreHistoryPageSearchState _restoreHistoryPageSearchState;
        private readonly GetRestoreHistoriesPageUseCase _getRestoresPageUseCase;

        private CancellationTokenSource? _cts;

        public RestoreHistoriesQueryCoordinator(IRestoreHistoryPageSearchState restoreHistoryPageSearchState, GetRestoreHistoriesPageUseCase getRestoresPageUseCase)
        {
            _restoreHistoryPageSearchState = restoreHistoryPageSearchState;
            _getRestoresPageUseCase = getRestoresPageUseCase;

            _restoreHistoryPageSearchState.BusyState.PropertyChanged += OnItemPageSearchStatePropertyChanged;
        }

        private void OnItemPageSearchStatePropertyChanged(object? _, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(_restoreHistoryPageSearchState.BusyState.QueryBusy)) CancelCommand.NotifyCanExecuteChanged();
        }

        private bool CanCancel() => _restoreHistoryPageSearchState.BusyState.QueryBusy;

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
            => UpdatePageSearchState(_restoreHistoryPageSearchState.ToGetRestoreHistoryPageInput(), external);

        /// <summary>
        /// 지정한 페이지로 이동
        /// </summary>
        public Task GoToPageAsync(int page, CancellationToken external = default)
            => UpdatePageSearchState(_restoreHistoryPageSearchState.GetrestoreHistoryPageInputFromNewPage(page), external);


        /// <summary>
        /// 새로운 필터로 검색
        /// </summary>
        public Task SearchWithFilterAsync(
            int page, int pageSize,
            DateTime? fromUtcFilter, DateTime? toUtcFilter, string? actorFilter, bool? dryOnlyFilter,
            CancellationToken external = default)
            => UpdatePageSearchState(new(new(page, pageSize), new(fromUtcFilter, toUtcFilter, actorFilter, dryOnlyFilter)), external);

        private async Task UpdatePageSearchState(GetRestoreHistoriesPageInput input, CancellationToken external)
        {
            if (input.Pagination.PageNumber == 0 || input.Pagination.PageSize == 0) { return; }

            var token = NewToken(external);
            var myCts = _cts;

            try
            {
                SetQueryBusy(true);
                var output = await _getRestoresPageUseCase.Handle(input, token);
                // 성공적으로 Get 한 경우에 업데이트
                _restoreHistoryPageSearchState.ReplacePageResults(output);
                _restoreHistoryPageSearchState.ReplaceFilter(
                    input.Filter?.FromUtc, input.Filter?.ToUtc, input.Filter?.Actor, input.Filter?.DryOnly);
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
            _restoreHistoryPageSearchState.BusyState.QueryBusy = value;
            CancelCommand.NotifyCanExecuteChanged();
        }

        public void Dispose()
        {
            _restoreHistoryPageSearchState.BusyState.PropertyChanged -= OnItemPageSearchStatePropertyChanged;
            try { _cts?.Cancel(); } catch { }
            _cts?.Dispose();
        }
    }
}
