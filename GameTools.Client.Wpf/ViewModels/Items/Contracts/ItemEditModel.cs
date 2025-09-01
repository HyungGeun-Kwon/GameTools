using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.ComponentModel;

namespace GameTools.Client.Wpf.ViewModels.Items.Contracts
{
    public partial class ItemEditModel : ObservableValidator, IEditableObject
    {
        private sealed record Snapshot(string Name, int Price, string? Description, byte RarityId);

        private Snapshot? _isDirtyBaseline;

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

        [ObservableProperty]
        private bool _isDirty;

        private string? _rowVersion;
        public string? RowVersionBase64 { get => _rowVersion; private set => SetProperty(ref _rowVersion, value); }

        private int? _id;
        public int? Id { get => _id; private set => SetProperty(ref _id, value); }

        /// <summary>
        /// 신규 추가를 위한 생성자
        /// </summary>
        public ItemEditModel()
        {
            Name = string.Empty;
            Price = 0;
            Description = null;
            RarityId = 0; // 미선택 → 검증에서 실패 상태

            ValidateAllProperties();
            IsDirty = true; // 신규는 기본적으로 변경됨 상태
        }


        /// <summary>
        /// 기존 데이터를 위한 생성자
        /// </summary>
        public ItemEditModel(
            int id,
            string name,
            int price,
            string? description,
            byte rarityId,
            string rowVersionBase64)
        {
            Id = id;
            Name = name;
            Price = price;
            Description = description;
            RarityId = rarityId;
            RowVersionBase64 = rowVersionBase64;

            ValidateAllProperties();
            ResetIsDirtyBaseline();
            IsDirty = false;
        }

        partial void OnNameChanged(string? oldValue, string newValue)
        {
            var normalized = (newValue ?? string.Empty).Trim();
            if (!ReferenceEquals(newValue, normalized) && newValue != normalized)
            {
                Name = normalized; return;
            }
            SetIsDirty();
        }

        partial void OnPriceChanged(int oldValue, int newValue) => SetIsDirty();

        partial void OnDescriptionChanged(string? oldValue, string? newValue)
        {
            var normalized = string.IsNullOrWhiteSpace(newValue) ? null : newValue!.Trim();
            if (!Equals(newValue, normalized))
            {
                Description = normalized; return;
            }
            SetIsDirty();
        }

        partial void OnRarityIdChanged(byte oldValue, byte newValue) => SetIsDirty();

        private Snapshot? _editBackup;

        public void BeginEdit()
        {
            _editBackup ??= new Snapshot(Name, Price, Description, RarityId);
        }

        public void CancelEdit()
        {
            if (_editBackup is null) return;

            // 복원
            Name = _editBackup.Name;
            Price = _editBackup.Price;
            Description = _editBackup.Description;
            RarityId = _editBackup.RarityId;

            _editBackup = null;

            // 재검증
            ValidateAllProperties();

            SetIsDirty();
        }

        public void EndEdit() => _editBackup = null;

        /// <summary>
        /// 수정 완료 시 반드시 호출
        /// </summary>
        public void FinishEdit(string newRowVersionBase64)
        {
            RowVersionBase64 = newRowVersionBase64;
            ResetIsDirtyBaseline();
            IsDirty = false;
        }

        /// <summary>
        /// 생성 완료 시 반드시 호출
        /// </summary>
        public void FinishCreate(int newId, string newRowVersionBase64)
        {
            Id = newId;
            RowVersionBase64 = newRowVersionBase64;
            ResetIsDirtyBaseline();
            IsDirty = false;
        }

        /// <summary>
        /// 저장 성공 후 Dirty 초기화
        /// </summary>
        public void AcceptChanges()
        {
            ResetIsDirtyBaseline();
            IsDirty = false;
        }

        private void SetIsDirty()
            => IsDirty = _isDirtyBaseline is null
                || !string.Equals(Name, _isDirtyBaseline.Name)
                || Price != _isDirtyBaseline.Price
                || !string.Equals(Description, _isDirtyBaseline.Description)
                || RarityId != _isDirtyBaseline.RarityId;

        private void ResetIsDirtyBaseline()
            => _isDirtyBaseline = new Snapshot(Name, Price, Description, RarityId);
    }
}
