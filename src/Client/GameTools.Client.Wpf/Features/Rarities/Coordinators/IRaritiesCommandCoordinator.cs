using CommunityToolkit.Mvvm.Input;
using GameTools.Client.Domain.Rarities;
using GameTools.Client.Wpf.Features.Rarities.Models;

namespace GameTools.Client.Wpf.Features.Rarities.Coordinators
{
    public interface IRaritiesCommandCoordinator : IDisposable
    {
        /// <summary>
        /// 진행 중인 Command 취소
        /// </summary>
        IRelayCommand CancelCommand { get; }

        Task<Rarity> UpdateAsync(RarityEditModel rarityEditModel, CancellationToken external = default);
        Task DeleteAsync(RarityEditModel rarityEditModel, CancellationToken external = default);
        Task<Rarity> CreateAsync(RarityCreateModel rarityEditModel, CancellationToken external = default);
    }
}
