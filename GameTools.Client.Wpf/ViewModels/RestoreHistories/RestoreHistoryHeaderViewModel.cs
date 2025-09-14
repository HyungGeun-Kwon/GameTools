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
        private int _searchPageNumber = PagingRules.DefaultPageNumber;

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Range(1, PagingRules.MaxPageSize)]
        private int _searchPageSize = PagingRules.DefaultPageSize;

        [ObservableProperty]
        private DateTime? _fromUtcFilter;
        
        [ObservableProperty]
        private DateTime? _toUtcFilter;

        [ObservableProperty]
        private string? _actorFilter;

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