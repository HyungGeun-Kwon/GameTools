using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace GameTools.Client.Wpf.Common.State
{
    public sealed partial class SearchState<T> : ObservableObject, ISearchState<T>
    {
        private readonly ObservableCollection<T> _inner = [];
        public ReadOnlyObservableCollection<T> Results { get; }
        public SearchState() => Results = new(_inner);

        [ObservableProperty]
        private bool _isBusy;

        [ObservableProperty]
        private Exception? _error;

        public void ReplaceResults(IEnumerable<T> items)
        {
            _inner.Clear();
            foreach (var i in items) _inner.Add(i);
        }

        public void Clear() => _inner.Clear();
    }
}
