using CommunityToolkit.Mvvm.ComponentModel;

namespace GameTools.Client.Wpf.Shared.State
{
    public partial class BusyState : ObservableObject
    {
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsBusy))]
        public partial bool QueryBusy { get; set; }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsBusy))]
        public partial bool CommandBusy { get; set; }

        public bool IsBusy => QueryBusy || CommandBusy;
    }
}
