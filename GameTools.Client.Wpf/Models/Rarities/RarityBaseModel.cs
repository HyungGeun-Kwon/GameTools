using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.ComponentModel;

namespace GameTools.Client.Wpf.Models.Rarities
{
    public abstract partial class RarityBaseModel : ObservableValidator
    {
        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required, MinLength(1)]
        [MaxLength(32)]
        private string _grade = string.Empty;

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required]
        [RegularExpression("^#[0-9A-F]{6}$", ErrorMessage = "Color must be '#RRGGBB' (uppercase).")]
        private string _colorCode = "#A0A0A0";

        protected RarityBaseModel() => ValidateAllProperties();

        partial void OnGradeChanged(string? oldValue, string newValue)
        {
            var normalized = newValue?.Trim() ?? string.Empty;
            if (normalized != newValue) { Grade = normalized; return; }
            AfterNormalizedChange(nameof(Grade), oldValue, newValue);
        }

        partial void OnColorCodeChanged(string? oldValue, string newValue)
        {
            var normalized = newValue?.Trim().ToUpperInvariant() ?? string.Empty;
            if (normalized != newValue) { ColorCode = normalized; return; }
            AfterNormalizedChange(nameof(ColorCode), oldValue, newValue);
        }

        protected virtual void AfterNormalizedChange(string propertyName, object? oldValue, object? newValue)
        {
        }
    }
}
