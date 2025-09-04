using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DotNetHelper.MsDiKit.Common;
using DotNetHelper.MsDiKit.RegionServices;
using GameTools.Client.Application.Common.Paging;
using GameTools.Client.Application.UseCases.Items.GetItemsPage;
using GameTools.Client.Domain.Items;
using GameTools.Client.Wpf.Common.State;
using GameTools.Client.Wpf.Models.Lookups;
using GameTools.Client.Wpf.ViewModels.Items.Mappers;

namespace GameTools.Client.Wpf.ViewModels.Items
{
    public partial class ItemSearchViewModel(
        IItemPageSearchState itemPageSearchState,
        GetItemsPageUseCase getItemsPageUseCase,
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
        [ObservableProperty]
        private RarityLookupViewModel _rarityLookup = rarityLookupViewModel;

        [RelayCommand(IncludeCancelCommand = true, AllowConcurrentExecutions = false)]
        private async Task GetItemsAsync(CancellationToken ct)
        {
            PagedOutput<Item> itemPageOutput = await getItemsPageUseCase.Handle(GetItemsPageInput(), ct);
            
            itemPageSearchState.ReplacePageResults(itemPageOutput.ToPagedItemEditModel());
            itemPageSearchState.ReplaceFilter(NameFilter, RarityIdFilter);
        }

        private GetItemsPageInput GetItemsPageInput()
        {
            var pagination = new Pagination(SearchPageNumber, SearchPageSize);
            var itemFilter = new ItemSearchFilter(NameFilter, RarityIdFilter);
            return new(pagination, itemFilter);
        }

        public async void OnRegionActivated(Parameters? parameters)
            => await RarityLookup.LoadCommand.ExecuteAsync(null);

        public void OnRegionDeactivated() { }
    }
}
