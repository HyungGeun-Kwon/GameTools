using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using GameTools.Client.Wpf.Models.Items;

namespace GameTools.Client.Wpf.ViewModels.Items.Contracts
{
    public sealed partial class ItemEditModel : ItemBaseModel, IEditableObject
    {
        private sealed record Snapshot(string Name, int Price, string? Description, byte RarityId);

        private Snapshot? _isDirtyBaseline;

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
            RarityId = 0;

            ValidateAllProperties();
            IsDirty = true;
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

        public void RevertToSaved()
        {
            if (_isDirtyBaseline is null) return;

            // 현재 편집 중이면 세션 취소
            _editBackup = null;

            // 마지막 저장 스냅샷으로 복원
            Name = _isDirtyBaseline.Name;
            Price = _isDirtyBaseline.Price;
            Description = _isDirtyBaseline.Description;
            RarityId = _isDirtyBaseline.RarityId;

            // 재검증 + Dirty 재계산
            ValidateAllProperties();
            SetIsDirty();
        }

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

        protected override void AfterNormalizedChange(string propertyName, object? oldValue, object? newValue)
        {
            base.AfterNormalizedChange(propertyName, oldValue, newValue);
            SetIsDirty();
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
