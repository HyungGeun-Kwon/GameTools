using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DotNetHelper.MsDiKit.Common;
using DotNetHelper.MsDiKit.RegionServices;
using GameTools.Client.Application.Common.Paging;
using GameTools.Client.Wpf.Features.Items.Audits.Coordinators;
using GameTools.Client.Wpf.Shared.Enums;

namespace GameTools.Client.Wpf.Features.Items.Audits.ViewModels
{
    public sealed partial class ItemAuditSearchViewModel(
        IItemAuditsQueryCoordinator itemAuditsQueryCoordinator
        ): ObservableValidator, IRegionViewModel
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
        [Range(1, int.MaxValue)]
        public partial int? ItemIdFilter { get; set; }

        [ObservableProperty]
        public partial AuditActionType? ActionFilter { get; set; } = AuditActionType.ALL;

        [ObservableProperty]
        public partial DateTime? FromUtcFilter { get; set; }

        [ObservableProperty]
        public partial DateTime? ToUtcFilter { get; set; }

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
