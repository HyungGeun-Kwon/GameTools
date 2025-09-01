using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.ComponentModel;

namespace GameTools.Client.Wpf.ViewModels.Rarities.Contracts
{
    public partial class RarityEditModel : ObservableValidator, IEditableObject
    {
        private sealed record Snapshot(string Grade, string ColorCode);

        private Snapshot? _isDirtyBaseline;

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required, MinLength(1)]
        [MaxLength(32)]
        private string _grade;

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required]
        [RegularExpression("^#[0-9A-F]{6}$", ErrorMessage = "Color must be '#RRGGBB' (uppercase).")]
        private string _colorCode;

        [ObservableProperty]
        private bool _isDirty;

        private string? _rowVersion;
        public string? RowVersionBase64 { get => _rowVersion; private set => SetProperty(ref _rowVersion, value); }

        private byte? _id;
        public byte? Id { get => _id; private set => SetProperty(ref _id, value); }

        /// <summary>
        /// 신규 추가를 위한 생성자
        /// </summary>
        public RarityEditModel()
        {
            Grade = string.Empty;
            ColorCode = "#A0A0A0";
            ValidateAllProperties();
            IsDirty = true;
        }

        /// <summary>
        /// 기존 데이터를 위한 생성자
        /// </summary>
        public RarityEditModel(byte id, string grade, string colorCode, string rowVersion)
        {
            Id = id;
            Grade = grade;
            ColorCode = colorCode;
            RowVersionBase64 = rowVersion;

            ValidateAllProperties();
            ResetIsDirtyBaseline();
            IsDirty = false;
        }

        partial void OnGradeChanged(string? oldValue, string newValue)
        {
            var normalized = newValue?.Trim() ?? string.Empty;
            if (normalized != newValue)
            {
                Grade = normalized; return;
            }
            SetIsDirty();
        }
        partial void OnColorCodeChanged(string? oldValue, string newValue)
        {
            var normalized = newValue?.Trim().ToUpperInvariant() ?? string.Empty;
            if (normalized != newValue)
            {
                ColorCode = normalized; return;
            }
            SetIsDirty();
        }


        private Snapshot? _editBackup;

        public void BeginEdit()
        {
            _editBackup ??= new Snapshot(Grade, ColorCode);
        }

        public void CancelEdit()
        {
            if (_editBackup is null) return;

            // 복원
            Grade = _editBackup.Grade;
            ColorCode = _editBackup.ColorCode;

            _editBackup = null;

            // 재검증
            ValidateAllProperties();

            SetIsDirty();
        }

        public void EndEdit() => _editBackup = null;

        /// <summary>
        /// 수정 완료 시 반드시 호출
        /// </summary>
        public void FinishEdit(string newRowVersion)
        {
            RowVersionBase64 = newRowVersion;
            ResetIsDirtyBaseline();
            IsDirty = false;
        }

        /// <summary>
        /// 생성 완료 시 반드시 호출
        /// </summary>
        public void FinishCreate(byte newId, string newRowVersion)
        {
            Id = newId;
            RowVersionBase64 = newRowVersion;
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
                || !string.Equals(Grade, _isDirtyBaseline.Grade)
                || !string.Equals(ColorCode, _isDirtyBaseline.ColorCode);

        private void ResetIsDirtyBaseline()
            => _isDirtyBaseline = new Snapshot(Grade, ColorCode);
    }
}
