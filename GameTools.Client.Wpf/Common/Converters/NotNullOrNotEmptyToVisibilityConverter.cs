using System.Globalization;
using System.Windows;

namespace GameTools.Client.Wpf.Common.Converters
{
    public sealed class NotNullOrNotEmptyToVisibilityConverter : MarkupValueConverterBase
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var text = value as string;
            if (!string.IsNullOrEmpty(text))
                return Visibility.Visible;

            return Visibility.Collapsed;
        }
    }
}
