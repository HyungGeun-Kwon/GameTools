using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace GameTools.Client.Wpf.Shared.Helpers
{
    public static class EnumDisplay
    {
        public static string GetText(Enum value)
        {
            var type = value.GetType();
            var name = Enum.GetName(type, value);
            if (name is null) return value.ToString();

            var field = type.GetField(name);
            if (field is null) return name;

            var display = field.GetCustomAttribute<DisplayAttribute>();
            if (!string.IsNullOrWhiteSpace(display?.GetName()))
                return display!.GetName()!;

            var desc = field.GetCustomAttribute<DescriptionAttribute>();
            if (!string.IsNullOrWhiteSpace(desc?.Description))
                return desc!.Description!;

            return name;
        }
    }
}
