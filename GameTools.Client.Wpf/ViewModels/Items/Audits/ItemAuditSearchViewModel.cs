using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DotNetHelper.MsDiKit.Common;
using DotNetHelper.MsDiKit.RegionServices;
using GameTools.Client.Application.Common.Paging;
using GameTools.Client.Wpf.Common.Coordinators.Audits;
using GameTools.Client.Wpf.Common.Enums;

namespace GameTools.Client.Wpf.ViewModels.Items.Audits
{
    public sealed partial class ItemAuditSearchViewModel(
        IItemAuditsQueryCoordinator itemAuditsQueryCoordinator
        ): ObservableValidator, IRegionViewModel
    {
        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Range(1, int.MaxValue)]
        private int _searchPageNumber = PagingRules.DefaultPageNumber;

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Range(1, PagingRules.MaxPageSize)]
        private int _searchPageSize = PagingRules.DefaultPageSize;

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Range(1, int.MaxValue)]
        private int? _itemIdFilter;

        [ObservableProperty]
        private AuditActionType? _actionFilter = AuditActionType.ALL;

        [ObservableProperty]
        private DateTime? _fromUtcFilter;

        [ObservableProperty]
        private DateTime? _toUtcFilter;

        [RelayCommand(AllowConcurrentExecutions = false)]
        private Task GetItemAuditsAsync()
            => itemAuditsQueryCoordinator.SearchWithFilterAsync(
                SearchPageNumber, 
                SearchPageSize,
                ItemIdFilter, 
                ActionFilter, 
                FromUtcFilter, 
                ToUtcFilter);

        public void OnRegionActivated(Parameters? parameters) { }
        public void OnRegionDeactivated() { }
    }
}
