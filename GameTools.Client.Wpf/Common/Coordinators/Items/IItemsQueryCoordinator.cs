using CommunityToolkit.Mvvm.Input;

namespace GameTools.Client.Wpf.Common.Coordinators.Items
{
    public interface IItemsQueryCoordinator : IDisposable
    {
        /// <summary>
        /// 진행 중인 조회 취소
        /// </summary>
        IRelayCommand CancelCommand { get; }

        /// <summary>
        /// 동일한 필터로 재검색 (Delete, Create 이후 등)
        /// </summary>
        Task RefreshAsync(CancellationToken external = default);

        /// <summary>
        /// 지정한 페이지로 이동
        /// </summary>
        Task GoToPageAsync(int page, CancellationToken external = default);

        /// <summary>
        /// 새로운 필터로 검색
        /// </summary>
        Task SearchWithFilterAsync(
            int page, int pageSize,
            string? nameFilter, byte? rarityIdFilter,
            CancellationToken external = default);
    }
}
