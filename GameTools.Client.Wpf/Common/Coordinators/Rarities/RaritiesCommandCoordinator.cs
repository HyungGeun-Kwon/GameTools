using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GameTools.Client.Application.UseCases.Rarities.CreateRarity;
using GameTools.Client.Application.UseCases.Rarities.DeleteRarity;
using GameTools.Client.Application.UseCases.Rarities.UpdateRarity;
using GameTools.Client.Domain.Rarities;
using GameTools.Client.Wpf.Common.State;
using GameTools.Client.Wpf.Models.Rarities;
using GameTools.Client.Wpf.ViewModels.Rarities.Contracts;
using GameTools.Client.Wpf.ViewModels.Rarities.Mappers;

namespace GameTools.Client.Wpf.Common.Coordinators.Rarities
{
    public sealed partial class RaritiesCommandCoordinator : ObservableObject, IRaritiesCommandCoordinator
    {
        private readonly ISearchState<RarityEditModel> _raritySearchState;
        private readonly UpdateRarityUseCase _updateRarityUseCase;
        private readonly DeleteRarityUseCase _deleteRarityUseCase;
        private readonly CreateRarityUseCase _createRarityUseCase;

        private CancellationTokenSource? _cts;

        public RaritiesCommandCoordinator(
            ISearchState<RarityEditModel> raritySearchState, 
            UpdateRarityUseCase updateRarityUseCase, 
            DeleteRarityUseCase deleteRarityUseCase, 
            CreateRarityUseCase createRarityUseCase)
        {
            _raritySearchState = raritySearchState;
            _updateRarityUseCase = updateRarityUseCase;
            _deleteRarityUseCase = deleteRarityUseCase;
            _createRarityUseCase = createRarityUseCase;

            _raritySearchState.BusyState.PropertyChanged += OnRaritySearchStatePropertyChanged;
        }

        private void OnRaritySearchStatePropertyChanged(object? _, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(_raritySearchState.BusyState.QueryBusy)) CancelCommand.NotifyCanExecuteChanged();
        }

        private bool CanCancel() => _raritySearchState.BusyState.QueryBusy;

        [RelayCommand(CanExecute = nameof(CanCancel))]
        private void Cancel() => _cts?.Cancel();

        private CancellationToken NewToken(CancellationToken external)
        {
            try { _cts?.Cancel(); } catch (ObjectDisposedException) { }
            _cts = CancellationTokenSource.CreateLinkedTokenSource(external);
            return _cts.Token;
        }

        public Task<Rarity> CreateAsync(RarityCreateModel rarityEditModel, CancellationToken external = default)
            => RunExclusiveCommandAsync(ct => _createRarityUseCase.Handle(rarityEditModel.ToCreateRarityInput(), ct), external);

        public Task DeleteAsync(RarityEditModel rarityEditModel, CancellationToken external = default)
            => RunExclusiveCommandAsync(ct => _deleteRarityUseCase.Handle(rarityEditModel.ToDeleteRarityInput(), ct), external);

        public Task<Rarity> UpdateAsync(RarityEditModel rarityEditModel, CancellationToken external = default)
            => RunExclusiveCommandAsync(ct => _updateRarityUseCase.Handle(rarityEditModel.ToUpdateRarityInput(), ct), external);

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
            _raritySearchState.BusyState.CommandBusy = value;
            CancelCommand.NotifyCanExecuteChanged();
        }

        public void Dispose()
        {
            _raritySearchState.BusyState.PropertyChanged -= OnRaritySearchStatePropertyChanged;

            try { _cts?.Cancel(); } catch { }
            _cts?.Dispose();
        }
    }
}
