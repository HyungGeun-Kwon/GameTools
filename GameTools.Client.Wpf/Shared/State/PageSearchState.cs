using CommunityToolkit.Mvvm.ComponentModel;
using GameTools.Client.Application.Common.Paging;

namespace GameTools.Client.Wpf.Shared.State
{
    public partial class PageSearchState<T> : SearchState<T>, IPageSearchState<T>
    {
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(StartIndex))]
        [NotifyPropertyChangedFor(nameof(EndIndex))]
        [NotifyPropertyChangedFor(nameof(HasPreview))]
        [NotifyPropertyChangedFor(nameof(HasNext))]
        [NotifyPropertyChangedFor(nameof(TotalPageNumber))]
        public partial int PageNumber { get; private set; } = 0;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(StartIndex))]
        [NotifyPropertyChangedFor(nameof(EndIndex))]
        [NotifyPropertyChangedFor(nameof(HasNext))]
        [NotifyPropertyChangedFor(nameof(TotalPageNumber))]
        public partial int PageSize { get; private set; } = 0;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(StartIndex))]
        [NotifyPropertyChangedFor(nameof(EndIndex))]
        [NotifyPropertyChangedFor(nameof(HasNext))]
        [NotifyPropertyChangedFor(nameof(TotalPageNumber))]
        public partial int TotalCount { get; private set; } = 0;

        public int TotalPageNumber => PageSize > 0 ? (int)Math.Ceiling((double)TotalCount / PageSize) : 0;
        public int StartIndex => TotalCount == 0 || PageSize <= 0 ? 0 : (Math.Max(1, PageNumber) - 1) * PageSize + 1;
        public int EndIndex => TotalCount == 0 || PageSize <= 0 ? 0 : Math.Min(Math.Max(1, PageNumber) * PageSize, TotalCount); 
        public bool HasPreview => PageNumber > 1;
        public bool HasNext => TotalPageNumber > 0 && PageNumber < TotalPageNumber;

        public void ReplacePageResults(PagedOutput<T> page)
        {
            ReplaceResults(page.Items);

            PageNumber = page.PageNumber;
            PageSize = page.PageSize;
            TotalCount = page.TotalCount;
        }
    }
}
