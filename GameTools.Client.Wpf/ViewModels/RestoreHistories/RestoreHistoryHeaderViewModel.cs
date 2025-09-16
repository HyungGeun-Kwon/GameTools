using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DotNetHelper.MsDiKit.Common;
using DotNetHelper.MsDiKit.RegionServices;
using GameTools.Client.Application.Common.Paging;
using GameTools.Client.Wpf.Common.Coordinators.RestoreHistories;

namespace GameTools.Client.Wpf.ViewModels.RestoreHistories
{
    public sealed partial class RestoreHistoryHeaderViewModel(
        IRestoreHistoriesQueryCoordinator restoreHistoryQueryCoordinator
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
        public partial DateTime? FromUtcFilter { get; set; }

        [ObservableProperty]
        public partial DateTime? ToUtcFilter { get; set; }

        [ObservableProperty]
        public partial string? ActorFilter { get; set; }

        [RelayCommand(AllowConcurrentExecutions = false)]
        private Task RestoreSearchAsync()
            => restoreHistoryQueryCoordinator.SearchWithFilterAsync(
                SearchPageNumber,
                SearchPageSize,
                FromUtcFilter,
                ToUtcFilter,
                ActorFilter,
                null);

        public void OnRegionActivated(Parameters? _) { }
        public void OnRegionDeactivated() { }
    }
}