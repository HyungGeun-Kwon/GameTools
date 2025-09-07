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

        public Task<Rarity> CreateAsync(RarityCreateModel rarityEditModel, bool throwCancelException = false, CancellationToken external = default)
            => RunExclusiveCommandAsync(ct => _createRarityUseCase.Handle(rarityEditModel.ToCreateRarityInput(), ct), throwCancelException, external);

        public Task DeleteAsync(RarityEditModel rarityEditModel, bool throwCancelException = false, CancellationToken external = default)
            => RunExclusiveCommandAsync(ct => _deleteRarityUseCase.Handle(rarityEditModel.ToDeleteRarityInput(), ct), throwCancelException, external);

        public Task<Rarity> UpdateAsync(RarityEditModel rarityEditModel, bool throwCancelException = false, CancellationToken external = default)
            => RunExclusiveCommandAsync(ct => _updateRarityUseCase.Handle(rarityEditModel.ToUpdateRarityInput(), ct), throwCancelException, external);

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
            _raritySearchState.BusyState.CommandBusy = value;
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
