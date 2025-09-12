using GameTools.Client.Application.Common.Paging;

namespace GameTools.Client.Wpf.Common.State
{
    public interface IPageState
    {
        int PageNumber { get; }
        int TotalPageNumber { get; }
        int PageSize { get; }
        int TotalCount { get; }

        int StartIndex { get; }
        int EndIndex { get; }
        bool HasPreview { get; }
        bool HasNext { get; }
    }
    public interface IPageSearchState<T> : ISearchState<T>, IPageState
    {
        void ReplacePageResults(PagedOutput<T> page);
    }
}
