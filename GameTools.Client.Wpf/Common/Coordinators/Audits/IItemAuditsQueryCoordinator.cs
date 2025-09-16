using CommunityToolkit.Mvvm.Input;
using GameTools.Client.Wpf.Common.Enums;

namespace GameTools.Client.Wpf.Common.Coordinators.Audits
{
    public interface IItemAuditsQueryCoordinator : IDisposable
    {
        IRelayCommand CancelCommand { get; }

        Task RefreshAsync(CancellationToken external = default);
        Task GoToPageAsync(int page, CancellationToken external = default);
        Task SearchWithFilterAsync(
            int page, int pageSize,
            int? itemId, AuditActionType? action, DateTime? fromUtc, DateTime? toUtc,
            CancellationToken external = default);
    }
}
