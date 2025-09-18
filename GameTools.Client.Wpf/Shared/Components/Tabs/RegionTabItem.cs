using CommunityToolkit.Mvvm.ComponentModel;
using DotNetHelper.MsDiKit.Common;

namespace GameTools.Client.Wpf.Shared.Components.Tabs
{
    public partial class RegionTabItem(string header, string viewKey, Parameters? parameters = null, string? regionName = null) : ObservableObject
    {
        public string Header { get; } = header;
        public string RegionName { get; } = regionName ?? $"Tab.{Guid.NewGuid()}";
        public string ViewKey { get; } = viewKey;
        public bool IsInitialized { get; set; } = false;

        [ObservableProperty]
        public partial Parameters? Parameters { get; private set; } = parameters; // 내부는 변경 감지하지 않음. 반드시 Parameter를 변경할것

        public void SetParameters(Parameters? parameters) => Parameters = parameters;
    }
}
