using System.Collections;
using System.Windows.Markup;
using GameTools.Client.Wpf.Common.Helpers;

namespace GameTools.Client.Wpf.Common.Markups
{
    [MarkupExtensionReturnType(typeof(IEnumerable))]
    public sealed class EnumValuesExtension : MarkupExtension
    {
        public Type EnumType { get; set; } = null!;
        public bool ExcludeDefault { get; set; } = false; // 0 값 빼고 싶을 때 true

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (EnumType == null) throw new InvalidOperationException("EnumType is required.");
            var type = Nullable.GetUnderlyingType(EnumType) ?? EnumType;
            if (!type.IsEnum) throw new ArgumentException("EnumType must be an enum type.");

            IEnumerable<Enum> values = Enum.GetValues(type).Cast<Enum>();
            if (ExcludeDefault)
                values = values.Where(v => Convert.ToInt64(v) != 0);

            return values.Select(v => new EnumValue { Value = v, Text = EnumDisplay.GetText(v) }).ToList();
        }

        public sealed class EnumValue
        {
            public Enum Value { get; init; } = default!;
            public string Text { get; init; } = string.Empty;
        }
    }
}
