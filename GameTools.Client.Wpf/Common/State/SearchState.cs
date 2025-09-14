using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace GameTools.Client.Wpf.Common.State
{
    public partial class SearchState<T> : ObservableObject, ISearchState<T>
    {
        private readonly ObservableCollection<T> _inner = [];
        public ReadOnlyObservableCollection<T> Results { get; }
        public BusyState BusyState { get; } = new();
        public SearchState() => Results = new(_inner);

        [ObservableProperty]
        public partial Exception? Error { get; set; }

        public void ReplaceResults(IEnumerable<T> items)
        {
            _inner.Clear();
            foreach (var i in items) _inner.Add(i);
        }

        public void Clear() => _inner.Clear();
    }
}
