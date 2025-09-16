using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.ComponentModel;
using GameTools.Client.Domain.Common.Rules;

namespace GameTools.Client.Wpf.Models.Rarities
{
    public abstract partial class RarityBaseModel : ObservableValidator
    {
        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required, MinLength(1)]
        [MaxLength(RarityRules.GradeMax)]
        public partial string Grade { get; set; } = string.Empty;

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required]
        [RegularExpression(RarityRules.ColorFormat, ErrorMessage = "Color must be '#RRGGBB' (uppercase).")]
        public partial string ColorCode { get; set; } = RarityRules.DefaultColor;

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
