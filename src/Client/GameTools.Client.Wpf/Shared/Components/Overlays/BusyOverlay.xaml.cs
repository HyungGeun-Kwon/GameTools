using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace GameTools.Client.Wpf.Shared.Components.Overlays
{
    /// <summary>
    /// BusyOverlay.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class BusyOverlay : UserControl
    {
        public BusyOverlay()
        {
            InitializeComponent();
        }

        // 표시/숨김
        public bool IsActive
        {
            get => (bool)GetValue(IsActiveProperty);
            set => SetValue(IsActiveProperty, value);
        }
        public static readonly DependencyProperty IsActiveProperty =
            DependencyProperty.Register(nameof(IsActive), typeof(bool), typeof(BusyOverlay), new PropertyMetadata(false));

        // 오버레이 클릭 차단 여부
        public bool BlockInput
        {
            get => (bool)GetValue(BlockInputProperty);
            set => SetValue(BlockInputProperty, value);
        }
        public static readonly DependencyProperty BlockInputProperty =
            DependencyProperty.Register(nameof(BlockInput), typeof(bool), typeof(BusyOverlay), new PropertyMetadata(true));

        // 크기/두께
        public double Size
        {
            get => (double)GetValue(SizeProperty);
            set => SetValue(SizeProperty, value);
        }
        public static readonly DependencyProperty SizeProperty =
            DependencyProperty.Register(nameof(Size), typeof(double), typeof(BusyOverlay), new PropertyMetadata(40d));

        public double Thickness
        {
            get => (double)GetValue(ThicknessProperty);
            set => SetValue(ThicknessProperty, value);
        }
        public static readonly DependencyProperty ThicknessProperty =
            DependencyProperty.Register(nameof(Thickness), typeof(double), typeof(BusyOverlay), new PropertyMetadata(4d));

        public Brush RingBrush
        {
            get => (Brush)GetValue(RingBrushProperty);
            set => SetValue(RingBrushProperty, value);
        }
        public static readonly DependencyProperty RingBrushProperty =
            DependencyProperty.Register(nameof(RingBrush), typeof(Brush), typeof(BusyOverlay),
                new PropertyMetadata(new SolidColorBrush(Color.FromRgb(0x00, 0x78, 0xD4))));

        public Brush RingBaseBrush
        {
            get => (Brush)GetValue(RingBaseBrushProperty);
            set => SetValue(RingBaseBrushProperty, value);
        }
        public static readonly DependencyProperty RingBaseBrushProperty =
            DependencyProperty.Register(nameof(RingBaseBrush), typeof(Brush), typeof(BusyOverlay),
                new PropertyMetadata(new SolidColorBrush(Color.FromArgb(0x33, 0x00, 0x00, 0x00))));

        public Brush OverlayBrush
        {
            get => (Brush)GetValue(OverlayBrushProperty);
            set => SetValue(OverlayBrushProperty, value);
        }
        public static readonly DependencyProperty OverlayBrushProperty =
            DependencyProperty.Register(nameof(OverlayBrush), typeof(Brush), typeof(BusyOverlay),
                new PropertyMetadata(Brushes.Transparent));

        // 취소 버튼
        public bool ShowCancel
        {
            get => (bool)GetValue(ShowCancelProperty);
            set => SetValue(ShowCancelProperty, value);
        }
        public static readonly DependencyProperty ShowCancelProperty =
            DependencyProperty.Register(nameof(ShowCancel), typeof(bool), typeof(BusyOverlay), new PropertyMetadata(false));

        public string CancelText
        {
            get => (string)GetValue(CancelTextProperty);
            set => SetValue(CancelTextProperty, value);
        }
        public static readonly DependencyProperty CancelTextProperty =
            DependencyProperty.Register(nameof(CancelText), typeof(string), typeof(BusyOverlay), new PropertyMetadata("Cancel"));

        public ICommand? CancelCommand
        {
            get => (ICommand?)GetValue(CancelCommandProperty);
            set => SetValue(CancelCommandProperty, value);
        }
        public static readonly DependencyProperty CancelCommandProperty =
            DependencyProperty.Register(nameof(CancelCommand), typeof(ICommand), typeof(BusyOverlay), new PropertyMetadata(null));

        public object? CancelCommandParameter
        {
            get => (object?)GetValue(CancelCommandParameterProperty);
            set => SetValue(CancelCommandParameterProperty, value);
        }
        public static readonly DependencyProperty CancelCommandParameterProperty =
            DependencyProperty.Register(nameof(CancelCommandParameter), typeof(object), typeof(BusyOverlay), new PropertyMetadata(null));
    }
}