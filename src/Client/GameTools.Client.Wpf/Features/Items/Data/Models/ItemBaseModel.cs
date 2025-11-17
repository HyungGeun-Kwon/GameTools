using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.ComponentModel;
using GameTools.Client.Domain.Common.Rules;

namespace GameTools.Client.Wpf.Features.Items.Data.Models
{
    public abstract partial class ItemBaseModel : ObservableValidator
    {
        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required, MinLength(1)]
        [MaxLength(ItemRules.NameMax)]
        public partial string Name { get; set; } = string.Empty;

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Range(0, int.MaxValue)]
        public partial int Price { get; set; }

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [StringLength(ItemRules.DescriptionMax)]
        public partial string? Description { get; set; }

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Range(1, byte.MaxValue, ErrorMessage = "Select a rarity.")]
        public partial byte RarityId { get; set; }

        protected ItemBaseModel() => ValidateAllProperties();

        partial void OnNameChanged(string oldValue, string newValue)
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
