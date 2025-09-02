using GameTools.Client.Application.Common.Paging;

namespace GameTools.Client.Wpf.Common.State
{
    public partial class PageSearchState<T> : SearchState<T>, IPageSearchState<T>
    {
        private int _pageNumber = 0;
        private int _totalPageNumber = 0;
        private int _pageSize = 0;
        private int _totalCount = 0;

        public int PageNumber { get => _pageNumber; private set => SetProperty(ref _pageNumber, value); }
        public int TotalPageNumber { get => _totalPageNumber; private set => SetProperty(ref _totalPageNumber, value);}
        public int PageSize { get => _pageSize; private set => SetProperty(ref _pageSize, value);}
        public int TotalCount { get => _totalCount; private set => SetProperty(ref _totalCount, value);}

        public void ReplacePageResults(PagedOutput<T> page)
        {
            ReplaceResults(page.Items);

            PageNumber = page.PageNumber;
            PageSize = page.PageSize;
            TotalCount = page.TotalCount;
            TotalPageNumber = PageSize > 0
                ? (int)Math.Ceiling((double)TotalCount / PageSize)
                : 0;
        }
    }
}
