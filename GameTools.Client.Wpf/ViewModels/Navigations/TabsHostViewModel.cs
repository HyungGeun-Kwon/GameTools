using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using GameTools.Client.Wpf.Models.Tabs;

namespace GameTools.Client.Wpf.ViewModels.Navigations
{
    public partial class TabsHostViewModel : ObservableObject
    {
        [ObservableProperty]
        public partial RegionTabItem? SelectedTab { get; set; }

        public event EventHandler<RegionTabItem?>? Navigating;
        public event EventHandler<RegionTabItem?>? Navigated;
        
        public ObservableCollection<RegionTabItem> Tabs { get; } = [];

        partial void OnSelectedTabChanging(RegionTabItem? value)
            => Navigating?.Invoke(this, value);

        partial void OnSelectedTabChanged(RegionTabItem? value)
            => Navigated?.Invoke(this, value);

        public void AddTab(RegionTabItem regionTabItem, bool setSelection = false)
        {
            Tabs.Add(regionTabItem);
            if (setSelection) SelectedTab = regionTabItem;
        }

        public void RemoveTab(RegionTabItem regionTabItem)
            => Tabs.Remove(regionTabItem);
    }
}
