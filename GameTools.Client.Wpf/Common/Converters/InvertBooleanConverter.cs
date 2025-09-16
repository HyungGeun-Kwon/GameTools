using System.Globalization;

namespace GameTools.Client.Wpf.Common.Converters
{
    public sealed class InvertBooleanConverter : MarkupValueConverterBase
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolVal) { return !boolVal; }
            return value;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolVal) { return !boolVal; }
            return value;
        }
    }
}
