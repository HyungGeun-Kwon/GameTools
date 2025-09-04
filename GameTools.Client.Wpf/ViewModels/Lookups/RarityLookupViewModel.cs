using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GameTools.Client.Application.UseCases.Rarities.GetAllRarities;
using GameTools.Client.Domain.Rarities;
using GameTools.Client.Wpf.Models.Lookups;

namespace GameTools.Client.Wpf.ViewModels.Items
{
    public sealed partial class RarityLookupViewModel(GetAllRaritiesUseCase getAllRaritiesUseCase) : ObservableObject
    {
        [ObservableProperty] 
        private bool _isLoading;
        [ObservableProperty] 
        private string? _error;

        public ObservableCollection<RarityOptionModel> Options { get; } = [];
        public ObservableCollection<RarityOptionModel> AllOptions { get; } = [];

        [RelayCommand(IncludeCancelCommand = true, AllowConcurrentExecutions = false)]
        private async Task LoadAsync(CancellationToken ct)
        {
            try
            {
                IsLoading = true;
                Error = null;

                Options.Clear();
                AllOptions.Clear();
                AllOptions.Add(RarityOptionModel.All());

                IReadOnlyList<Rarity> rarities = await getAllRaritiesUseCase.Handle(ct);
                foreach (var r in rarities)
                {
                    Options.Add(r.ToOption());
                    AllOptions.Add(r.ToOption());
                }
            }
            catch (OperationCanceledException) { }
            catch (Exception ex) { Error = ex.Message; }
            finally { IsLoading = false; }
        }
    }
}
