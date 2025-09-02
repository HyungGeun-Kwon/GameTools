using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.ComponentModel;

namespace GameTools.Client.Wpf.Models.Items
{
    public abstract partial class ItemBaseModel : ObservableValidator
    {
        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required, MinLength(1)]
        [MaxLength(100)]
        private string _name = string.Empty;

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Range(0, int.MaxValue)]
        private int _price;

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [StringLength(500)]
        private string? _description;

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Range(1, byte.MaxValue, ErrorMessage = "Select a rarity.")]
        private byte _rarityId;

        protected ItemBaseModel() => ValidateAllProperties();

        partial void OnNameChanged(string? oldValue, string newValue)
        {
            var normalized = (newValue ?? string.Empty).Trim();
            if (!ReferenceEquals(newValue, normalized) && newValue != normalized)
            {
                Name = normalized; return;
            }
            AfterNormalizedChange(nameof(Name), oldValue, newValue);
        }

        partial void OnDescriptionChanged(string? oldValue, string? newValue)
        {
            var normalized = string.IsNullOrWhiteSpace(newValue) ? null : newValue!.Trim();
            if (!Equals(newValue, normalized))
            {
                Description = normalized; return;
            }
            AfterNormalizedChange(nameof(Description), oldValue, newValue);
        }

        partial void OnPriceChanged(int oldValue, int newValue) => AfterNormalizedChange(nameof(Price), oldValue, newValue);

        partial void OnRarityIdChanged(byte oldValue, byte newValue) => AfterNormalizedChange(nameof(RarityId), oldValue, newValue);

        protected virtual void AfterNormalizedChange(string propertyName, object? oldValue, object? newValue)
        {
        }
    }
}
