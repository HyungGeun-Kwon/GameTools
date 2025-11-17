using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace GameTools.Client.Wpf.Shared.Coordinators
{
    public abstract partial class CoordinatorBase : ObservableObject, IDisposable
    {
        private bool _disposed;

        private readonly INotifyPropertyChanged _busyNotifier;
        private readonly string _busyPropertyName;
        private readonly Func<bool> _isBusy;
        private readonly Action<bool> _setBusy;

        protected CancellationTokenSource? _cts;

        protected CoordinatorBase(
            INotifyPropertyChanged busyNotifier,
            string busyPropertyName,
            Func<bool> isBusy,
            Action<bool> setBusy)
        {
            _busyNotifier = busyNotifier;
            _busyPropertyName = busyPropertyName;
            _isBusy = isBusy;
            _setBusy = setBusy;

            _busyNotifier.PropertyChanged += OnBusyPropertyChanged;
        }

        private void OnBusyPropertyChanged(object? _, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == _busyPropertyName)
                CancelCommand.NotifyCanExecuteChanged();
        }

        private bool CanCancel() => _isBusy();

        [RelayCommand(CanExecute = nameof(CanCancel))]
        private void Cancel() => _cts?.Cancel();

        protected Task RunExclusiveAsync(Func<CancellationToken, Task> action, CancellationToken external = default)
            => RunExclusiveCoreAsync(async ct => { await action(ct); return true; }, external);

        protected Task<TResult> RunExclusiveAsync<TResult>(Func<CancellationToken, Task<TResult>> action, CancellationToken external = default)
            => RunExclusiveCoreAsync(action, external);

        private async Task<TResult> RunExclusiveCoreAsync<TResult>(
            Func<CancellationToken, Task<TResult>> action,
            CancellationToken external)
        {
            try { _cts?.Cancel(); } catch (ObjectDisposedException) { }
            _cts = CancellationTokenSource.CreateLinkedTokenSource(external);
            var myCts = _cts;

            try
            {
                _setBusy(true);
                return await action(myCts.Token);
            }
            finally
            {
                if (ReferenceEquals(myCts, _cts))
                {
                    _setBusy(false);
                    _cts = null;
                }
                myCts?.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;
            if (disposing)
            {
                _busyNotifier.PropertyChanged -= OnBusyPropertyChanged;
                try { _cts?.Cancel(); } catch { }
                _cts?.Dispose();
            }
            _disposed = true;
        }
    }
}
