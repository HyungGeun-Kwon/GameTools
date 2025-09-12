using CommunityToolkit.Mvvm.ComponentModel;
using DotNetHelper.MsDiKit.Common;
using DotNetHelper.MsDiKit.DialogServices;

namespace GameTools.Client.Wpf.ViewModels.Items.Audits
{
    public sealed class ItemAuditRestoreAsOfViewModel : ObservableObject, IDialogViewModel
    {
        public event Action<IDialogResult>? RequestClose;

        public void OnDialogClosed()
        {
        }

        public void OnDialogOpened(Parameters? parameters)
        {
        }
    }
}
