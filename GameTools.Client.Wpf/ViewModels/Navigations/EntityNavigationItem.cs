using DotNetHelper.MsDiKit.Common;

namespace GameTools.Client.Wpf.ViewModels.Navigations
{
    public sealed record EntityNavigationItem(string EntityName, string ViewName, Parameters? Parameters = null);

}
