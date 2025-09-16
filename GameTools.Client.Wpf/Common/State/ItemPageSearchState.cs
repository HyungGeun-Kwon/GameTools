using GameTools.Client.Wpf.ViewModels.Items.Contracts;

namespace GameTools.Client.Wpf.Common.State
{
    public sealed class ItemPageSearchState : PageSearchState<ItemEditModel>, IItemPageSearchState
    {
        public string? NameFilter { get; private set; }

        public byte? RarityIdFilter { get; private set; }

        public void ReplaceFilter(string? nameFilter, byte? rarityIdFilter)
        {
            NameFilter = nameFilter;
            RarityIdFilter = rarityIdFilter;
        }
    }
}
