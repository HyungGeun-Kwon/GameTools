using GameTools.Client.Wpf.Features.Items.Data.Models;
using GameTools.Client.Wpf.Shared.State;

namespace GameTools.Client.Wpf.Features.Items.Data.State
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
