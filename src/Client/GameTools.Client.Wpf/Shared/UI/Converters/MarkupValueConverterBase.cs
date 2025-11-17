using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace GameTools.Client.Wpf.Shared.UI.Converters
{
    [MarkupExtensionReturnType(typeof(IValueConverter))]
    public abstract class MarkupValueConverterBase : MarkupExtension, IValueConverter
    {
        /// <summary>
        /// XAML에서 이 컨버터를 사용할 때마다 파서가 만든 이 인스턴스를 그대로 반환.
        /// </summary>
        public sealed override object ProvideValue(IServiceProvider serviceProvider) => this;

        public abstract object Convert(object value, Type targetType, object parameter, CultureInfo culture);

        /// <summary>
        /// 기본은 역변환을 수행하지 않음 (Binding.DoNothing). 필요하면 파생 클래스에서 override 하여 구현
        /// </summary>
        public virtual object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => Binding.DoNothing;
    }
}
