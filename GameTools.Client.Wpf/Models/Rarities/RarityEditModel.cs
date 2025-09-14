using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using GameTools.Client.Wpf.Models.Rarities;

namespace GameTools.Client.Wpf.ViewModels.Rarities.Contracts
{
    public sealed partial class RarityEditModel : RarityBaseModel, IEditableObject
    {
        private sealed record Snapshot(string Grade, string ColorCode);

        private Snapshot? _isDirtyBaseline;
        private Snapshot? _editBackup;

        [ObservableProperty]
        public partial bool IsDirty { get; set; }
        [ObservableProperty]
        public partial string? RowVersionBase64 { get; private set; }

        [ObservableProperty]
        public partial byte? Id { get; private set; }

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

        public void RevertToSaved()
        {
            if (_isDirtyBaseline is null) return;

            // 현재 편집 중이면 세션 취소
            _editBackup = null;

            // 마지막 저장 스냅샷으로 복원
            Grade = _isDirtyBaseline.Grade;
            ColorCode = _isDirtyBaseline.ColorCode;

            // 재검증 + Dirty 재계산
            ValidateAllProperties();
            SetIsDirty();
        }

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
        /// 저장 성공 후 Dirty 초기화
        /// </summary>
        public void AcceptChanges()
        {
            ResetIsDirtyBaseline();
            IsDirty = false;
        }

        protected override void AfterNormalizedChange(string propertyName, object? oldValue, object? newValue)
        {
            base.AfterNormalizedChange(propertyName, oldValue, newValue);
            SetIsDirty();
        }

        private void SetIsDirty()
            => IsDirty = _isDirtyBaseline is null
                || !string.Equals(Grade, _isDirtyBaseline.Grade)
                || !string.Equals(ColorCode, _isDirtyBaseline.ColorCode);

        private void ResetIsDirtyBaseline()
            => _isDirtyBaseline = new Snapshot(Grade, ColorCode);
    }
}
