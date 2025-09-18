using System.Windows;
using System.Windows.Controls;

namespace GameTools.Client.Wpf.Shared.Components.Lookups.Rarities
{
    /// <summary>
    /// RarityFilterControl.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class RarityFilterControl : UserControl
    {
        public RarityLookupViewModel RarityLookup
        {
            get => (RarityLookupViewModel)GetValue(RarityLookupProperty);
            set => SetValue(RarityLookupProperty, value);
        }

        public static readonly DependencyProperty RarityLookupProperty =
            DependencyProperty.Register(
                nameof(RarityLookup),
                typeof(RarityLookupViewModel),
                typeof(RarityFilterControl),
                new PropertyMetadata(null));

        public byte? SelectedRaityId
        {
            get => (byte?)GetValue(SelectedRaityIdProperty);
            set => SetValue(SelectedRaityIdProperty, value);
        }

        public static readonly DependencyProperty SelectedRaityIdProperty =
            DependencyProperty.Register(
                nameof(SelectedRaityId),
                typeof(byte?),
                typeof(RarityFilterControl),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public bool UseAllOptions
        {
            get => (bool)GetValue(UseAllOptionsProperty);
            set => SetValue(UseAllOptionsProperty, value);
        }

        public static readonly DependencyProperty UseAllOptionsProperty =
            DependencyProperty.Register(
                nameof(UseAllOptions),
                typeof(bool),
                typeof(RarityFilterControl),
                new PropertyMetadata(false));

        public bool ShowErrorMessage
        {
            get => (bool)GetValue(ShowErrorMessageProperty);
            set => SetValue(ShowErrorMessageProperty, value);
        }

        public static readonly DependencyProperty ShowErrorMessageProperty =
            DependencyProperty.Register(
                nameof(ShowErrorMessage),
                typeof(bool),
                typeof(RarityFilterControl),
                new PropertyMetadata(false));

        public bool ShowRetryIfError
        {
            get => (bool)GetValue(ShowRetryIfErrorProperty);
            set => SetValue(ShowRetryIfErrorProperty, value);
        }

        public static readonly DependencyProperty ShowRetryIfErrorProperty =
            DependencyProperty.Register(
                nameof(ShowRetryIfError),
                typeof(bool),
                typeof(RarityFilterControl),
                new PropertyMetadata(false));

        public RarityFilterControl()
        {
            InitializeComponent();
        }
    }
}
