using DotNetHelper.MsDiKit.Common;

namespace GameTools.Client.Wpf.Features.Navigations.ViewModels
{
    public sealed record EntityNavigationItem(string EntityName, string ViewName, Parameters? Parameters = null);

}
