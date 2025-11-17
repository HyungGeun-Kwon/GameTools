using GameTools.Client.Wpf.Shared.State;
using GameTools.Client.Wpf.Features.Items.Data.Models;

namespace GameTools.Client.Wpf.Features.Items.Data.State
{
    public interface IItemPageSearchState : IPageSearchState<ItemEditModel>
    {
        string? NameFilter { get; }
        byte? RarityIdFilter { get; }
        void ReplaceFilter(string? nameFilter, byte? rarityIdFilter);
    }
}
