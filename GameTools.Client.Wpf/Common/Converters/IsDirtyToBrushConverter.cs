using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace GameTools.Client.Wpf.Common.Converters
{
    public sealed class IsDirtyToBrushConverter : MarkupValueConverterBase
    {
        private static readonly Brush DirtyBrush = Brushes.DarkOrange;
        private static readonly Brush LatestBrush = Brushes.MediumSeaGreen;

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isDirty)
                return isDirty ? DirtyBrush : LatestBrush;

            return Binding.DoNothing;
        }
    }
}
