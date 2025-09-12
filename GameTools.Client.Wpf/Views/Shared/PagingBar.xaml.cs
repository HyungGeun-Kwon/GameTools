using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using GameTools.Client.Wpf.Common.State;

namespace GameTools.Client.Wpf.Views.Shared
{
    /// <summary>
    /// PagingBar.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class PagingBar : UserControl
    {
        public PagingBar()
        {
            InitializeComponent();
        }

        public IPageState? PageState
        {
            get => (IPageState?)GetValue(PageStateProperty);
            set => SetValue(PageStateProperty, value);
        }
        public static readonly DependencyProperty PageStateProperty =
            DependencyProperty.Register(nameof(PageState), typeof(IPageState), typeof(PagingBar), new PropertyMetadata(null));

        public ICommand? GoFirstPageCommand
        {
            get => (ICommand?)GetValue(GoFirstPageCommandProperty);
            set => SetValue(GoFirstPageCommandProperty, value);
        }
        public static readonly DependencyProperty GoFirstPageCommandProperty =
            DependencyProperty.Register(nameof(GoFirstPageCommand), typeof(ICommand), typeof(PagingBar), new PropertyMetadata(null));

        public ICommand? GoPreviewPageCommand
        {
            get => (ICommand?)GetValue(GoPreviewPageCommandProperty);
            set => SetValue(GoPreviewPageCommandProperty, value);
        }
        public static readonly DependencyProperty GoPreviewPageCommandProperty =
            DependencyProperty.Register(nameof(GoPreviewPageCommand), typeof(ICommand), typeof(PagingBar), new PropertyMetadata(null));

        public ICommand? GoNextPageCommand
        {
            get => (ICommand?)GetValue(GoNextPageCommandProperty);
            set => SetValue(GoNextPageCommandProperty, value);
        }
        public static readonly DependencyProperty GoNextPageCommandProperty =
            DependencyProperty.Register(nameof(GoNextPageCommand), typeof(ICommand), typeof(PagingBar), new PropertyMetadata(null));

        public ICommand? GoLastPageCommand
        {
            get => (ICommand?)GetValue(GoLastPageCommandProperty);
            set => SetValue(GoLastPageCommandProperty, value);
        }
        public static readonly DependencyProperty GoLastPageCommandProperty =
            DependencyProperty.Register(nameof(GoLastPageCommand), typeof(ICommand), typeof(PagingBar), new PropertyMetadata(null));

        public ICommand? RefreshPageCommand
        {
            get => (ICommand?)GetValue(RefreshPageCommandProperty);
            set => SetValue(RefreshPageCommandProperty, value);
        }
        public static readonly DependencyProperty RefreshPageCommandProperty =
            DependencyProperty.Register(nameof(RefreshPageCommand), typeof(ICommand), typeof(PagingBar), new PropertyMetadata(null));
    }
}
