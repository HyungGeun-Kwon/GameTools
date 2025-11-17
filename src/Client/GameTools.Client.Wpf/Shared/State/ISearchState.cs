using System.Collections.ObjectModel;

namespace GameTools.Client.Wpf.Shared.State
{
    public interface ISearchState<T>
    {
        ReadOnlyObservableCollection<T> Results { get; }
        BusyState BusyState { get; }
        void ReplaceResults(IEnumerable<T> items);
        void Clear();
    }
}
