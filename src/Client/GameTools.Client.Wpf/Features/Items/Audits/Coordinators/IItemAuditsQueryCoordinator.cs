using CommunityToolkit.Mvvm.Input;
using GameTools.Client.Wpf.Shared.Enums;

namespace GameTools.Client.Wpf.Features.Items.Audits.Coordinators
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
