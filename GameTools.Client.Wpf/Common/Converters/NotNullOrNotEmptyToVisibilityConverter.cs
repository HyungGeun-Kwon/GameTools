using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace GameTools.Client.Wpf.Common.Converters
{
    public sealed class NotNullOrNotEmptyToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var text = value as string;
            if (!string.IsNullOrEmpty(text))
                return Visibility.Visible;

            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotSupportedException("NotNullOrNotEmptyToVisibilityConverter > ConvertBack");
    }
}
