using CommunityToolkit.Mvvm.Input;
using GameTools.Client.Wpf.ViewModels.Items.Contracts;

namespace GameTools.Client.Wpf.Common.Coordinators.Items
{
    public interface IItemsCsvCommandCoordinator
    {
        /// <summary>
        /// 진행 중인 Command 취소
        /// </summary>
        IRelayCommand CancelCommand { get; }
        Task ExportPageResultsAsync(IEnumerable<ItemEditModel> items, IEnumerable<string>? includeColumns = null, CancellationToken external = default);
        Task ExportBulkInsertTemplateAsync(CancellationToken external = default);
        Task ExportBulkUpdateTemplateAsync(CancellationToken external = default);
        Task ExportBulkDeleteTemplateAsync(CancellationToken external = default);
        Task ImportAndBulkInsertAsync(CancellationToken external = default);
        Task ImportAndBulkUpdateAsync(CancellationToken external = default);
        Task ImportAndBulkDeleteAsync(CancellationToken external = default);
        Task ImportAndAllResultDeleteAsync(CancellationToken external = default);
    }
}
