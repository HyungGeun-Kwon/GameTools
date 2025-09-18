using CommunityToolkit.Mvvm.Input;

namespace GameTools.Client.Wpf.Features.Rarities.Coordinators
{
    public interface IRaritiesQueryCoordinator : IDisposable
    {
        /// <summary>
        /// 진행 중인 조회 취소
        /// </summary>
        IRelayCommand CancelCommand { get; }

        /// <summary>
        /// 모든 Rarity 조회
        /// </summary>
        Task SearchAllAsync(CancellationToken external = default);
    }
}
