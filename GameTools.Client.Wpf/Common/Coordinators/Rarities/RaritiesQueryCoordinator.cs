using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GameTools.Client.Application.UseCases.Rarities.GetAllRarities;
using GameTools.Client.Wpf.Common.Coordinators.Rarities;
using GameTools.Client.Wpf.Common.State;
using GameTools.Client.Wpf.ViewModels.Rarities.Contracts;
using GameTools.Client.Wpf.ViewModels.Rarities.Mappers;

namespace GameTools.Client.Wpf.Common.Coordinators.Items
{
    public sealed partial class RaritiesQueryCoordinator : ObservableObject, IRaritiesQueryCoordinator
    {
        private readonly ISearchState<RarityEditModel> _raritySearchState;
        private readonly GetAllRaritiesUseCase _getRaritiesUseCase;

        private CancellationTokenSource? _cts;

        public RaritiesQueryCoordinator(ISearchState<RarityEditModel> raritySearchState, GetAllRaritiesUseCase getRaritiesPageUseCase)
        {
            _raritySearchState = raritySearchState;
            _getRaritiesUseCase = getRaritiesPageUseCase;

            _raritySearchState.BusyState.PropertyChanged += OnRaritySearchStatePropertyChanged;
        }

        private void OnRaritySearchStatePropertyChanged(object? _, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "QueryBusy") CancelCommand.NotifyCanExecuteChanged();
        }

        private bool CanCancel() => _raritySearchState.BusyState.QueryBusy;

        [RelayCommand(CanExecute = nameof(CanCancel))]
        private void Cancel() => _cts?.Cancel();

        private CancellationToken NewToken(CancellationToken external)
        {
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = CancellationTokenSource.CreateLinkedTokenSource(external);
            return _cts.Token;
        }

        public async Task SearchAllAsync(CancellationToken external = default)
        {
            var token = NewToken(external);
            var myCts = _cts;

            try
            {
                SetCommandBusy(true);
                var result = await _getRaritiesUseCase.Handle(token);
                _raritySearchState.ReplaceResults(result.ToEditModels());
            }
            catch (OperationCanceledException) { }
            finally
            {
                if (ReferenceEquals(myCts, _cts))
                    SetCommandBusy(false);
            }
        }

        private void SetCommandBusy(bool value)
        {
            _raritySearchState.BusyState.QueryBusy = value;
            CancelCommand.NotifyCanExecuteChanged();
        }

        public void Dispose()
        {
            _raritySearchState.BusyState.PropertyChanged -= OnRaritySearchStatePropertyChanged;
            _cts?.Cancel();
            _cts?.Dispose();
        }
    }
}
