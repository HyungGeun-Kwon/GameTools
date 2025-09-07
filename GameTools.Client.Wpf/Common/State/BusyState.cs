using CommunityToolkit.Mvvm.ComponentModel;

namespace GameTools.Client.Wpf.Common.State
{
    public partial class BusyState : ObservableObject
    {
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsBusy))]
        private bool _queryBusy;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsBusy))]
        private bool _commandBusy;

        public bool IsBusy => QueryBusy || CommandBusy;
    }
}
