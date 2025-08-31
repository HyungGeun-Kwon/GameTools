using System.Collections.ObjectModel;

namespace GameTools.Client.Wpf.Common.State
{
    public interface ISearchState<T>
    {
        ReadOnlyObservableCollection<T> Results { get; }
        bool IsBusy { get; set; }
        void ReplaceResults(IEnumerable<T> items);
        void Clear();
    }
}
