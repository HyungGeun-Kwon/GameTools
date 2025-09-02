using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DotNetHelper.MsDiKit.Common;
using DotNetHelper.MsDiKit.RegionServices;
using GameTools.Client.Application.Common.Paging;
using GameTools.Client.Application.UseCases.Items.GetItemsPage;
using GameTools.Client.Domain.Items;
using GameTools.Client.Wpf.Common.State;
using GameTools.Client.Wpf.ViewModels.Items.Mappers;

namespace GameTools.Client.Wpf.ViewModels.Items
{
    public partial class ItemSearchViewModel(
        IItemPageSearchState itemPageSearchState,
        GetItemsPageUseCase getItemsPageUseCase
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

        public void OnRegionActivated(Parameters? parameters) { }
        public void OnRegionDeactivated() { }
    }
}
