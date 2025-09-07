using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DotNetHelper.MsDiKit.Common;
using DotNetHelper.MsDiKit.RegionServices;
using GameTools.Client.Wpf.Common.Coordinators.Items;

namespace GameTools.Client.Wpf.ViewModels.Items
{
    public partial class ItemSearchViewModel(
        IItemsQueryCoordinator itemsQueryCoordinator,
        RarityLookupViewModel rarityLookupViewModel
        ) : ObservableObject, IRegionViewModel
    {
        [ObservableProperty]
        private int _searchPageNumber = 1;
        [ObservableProperty]
        private int _searchPageSize = 20;
        [ObservableProperty]
        private string? _nameFilter;
        [ObservableProperty]
        private byte? _rarityIdFilter;

        public RarityLookupViewModel RarityLookup => rarityLookupViewModel;

        [RelayCommand(AllowConcurrentExecutions = false)]
        private Task GetItemsAsync()
            => itemsQueryCoordinator.SearchWithFilterAsync(SearchPageNumber, SearchPageSize, NameFilter, RarityIdFilter);

        public async void OnRegionActivated(Parameters? parameters)
            => await RarityLookup.LoadCommand.ExecuteAsync(null);

        public void OnRegionDeactivated() { }
    }
}
