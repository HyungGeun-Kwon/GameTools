using CommunityToolkit.Mvvm.Input;
using GameTools.Client.Domain.Rarities;
using GameTools.Client.Wpf.Models.Rarities;
using GameTools.Client.Wpf.ViewModels.Rarities.Contracts;

namespace GameTools.Client.Wpf.Common.Coordinators.Rarities
{
    public interface IRaritiesCommandCoordinator : IDisposable
    {
        /// <summary>
        /// 진행 중인 Command 취소
        /// </summary>
        IRelayCommand CancelCommand { get; }

        Task<Rarity> UpdateAsync(RarityEditModel rarityEditModel, bool throwCancelException = false, CancellationToken external = default);
        Task DeleteAsync(RarityEditModel rarityEditModel, bool throwCancelException = false, CancellationToken external = default);
        Task<Rarity> CreateAsync(RarityCreateModel rarityEditModel, bool throwCancelException = false, CancellationToken external = default);
    }
}
