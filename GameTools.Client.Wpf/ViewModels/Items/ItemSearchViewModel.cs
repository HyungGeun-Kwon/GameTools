using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DotNetHelper.MsDiKit.Common;
using DotNetHelper.MsDiKit.RegionServices;
using GameTools.Client.Application.Common.Paging;
using GameTools.Client.Domain.Common.Rules;
using GameTools.Client.Wpf.Common.Coordinators.Items;

namespace GameTools.Client.Wpf.ViewModels.Items
{
    public partial class ItemSearchViewModel(
        IItemsQueryCoordinator itemsQueryCoordinator,
        RarityLookupViewModel rarityLookupViewModel
        ) : ObservableValidator, IRegionViewModel
    {
        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Range(1, PagingRules.MaxPageSize)]
        private int _searchPageNumber = PagingRules.DefaultPageNumber;

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Range(1, PagingRules.MaxPageSize)]
        private int _searchPageSize = PagingRules.DefaultPageSize;

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [StringLength(ItemRules.NameMax)]
        private string? _nameFilter;

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Range(1, byte.MaxValue, ErrorMessage = "Select a rarity.")]
        private byte? _rarityIdFilter;

        public RarityLookupViewModel RarityLookup => rarityLookupViewModel;

        [RelayCommand(AllowConcurrentExecutions = false)]
        private Task GetItemsAsync()
            => itemsQueryCoordinator.SearchWithFilterAsync(SearchPageNumber, SearchPageSize, NameFilter, RarityIdFilter);

        public async void OnRegionActivated(Parameters? parameters)
            => await RarityLookup.LoadCommand.ExecuteAsync(null);

        public void OnRegionDeactivated() => RarityLookup.LoadCancelCommand.Execute(null);
    }
}
