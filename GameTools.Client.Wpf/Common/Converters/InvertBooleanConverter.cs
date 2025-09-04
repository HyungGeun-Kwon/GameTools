using System.Globalization;
using System.Windows.Data;

namespace GameTools.Client.Wpf.Common.Converters
{
    public sealed class InvertBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolVal) { return !boolVal; }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolVal) { return !boolVal; }
            return value;
        }
    }
}
