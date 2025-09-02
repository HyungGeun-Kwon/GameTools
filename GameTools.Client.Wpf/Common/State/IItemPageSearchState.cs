using GameTools.Client.Wpf.ViewModels.Items.Contracts;

namespace GameTools.Client.Wpf.Common.State
{
    public interface IItemPageSearchState : IPageSearchState<ItemEditModel>
    {
        string? NameFilter { get; }
        byte? RarityIdFilter { get; }
        void ReplaceFilter(string? nameFilter, byte? rarityIdFilter);
    }
}
