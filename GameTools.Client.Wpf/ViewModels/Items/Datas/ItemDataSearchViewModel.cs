using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DotNetHelper.MsDiKit.Common;
using DotNetHelper.MsDiKit.RegionServices;
using GameTools.Client.Application.Common.Paging;
using GameTools.Client.Domain.Common.Rules;
using GameTools.Client.Wpf.Common.Coordinators.Items;

namespace GameTools.Client.Wpf.ViewModels.Items.Datas
{
    public sealed partial class ItemDataSearchViewModel(
        IItemsQueryCoordinator itemsQueryCoordinator,
        RarityLookupViewModel rarityLookupViewModel
        ) : ObservableValidator, IRegionViewModel
    {
        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Range(1, int.MaxValue)]
        public partial int SearchPageNumber { get; set; } = PagingRules.DefaultPageNumber;

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Range(1, PagingRules.MaxPageSize)]
        public partial int SearchPageSize { get; set; } = PagingRules.DefaultPageSize;

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [StringLength(ItemRules.NameMax)]
        public partial string? NameFilter { get; set; }

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Range(1, byte.MaxValue, ErrorMessage = "Select a rarity.")]
        public partial byte? RarityIdFilter { get; set; }

        public RarityLookupViewModel RarityLookup => rarityLookupViewModel;

        [RelayCommand(AllowConcurrentExecutions = false)]
        private Task GetItemsAsync()
            => itemsQueryCoordinator.SearchWithFilterAsync(SearchPageNumber, SearchPageSize, NameFilter, RarityIdFilter);

        public async void OnRegionActivated(Parameters? parameters)
            => await RarityLookup.LoadCommand.ExecuteAsync(null);

        public void OnRegionDeactivated() => RarityLookup.LoadCancelCommand.Execute(null);
    }
}
