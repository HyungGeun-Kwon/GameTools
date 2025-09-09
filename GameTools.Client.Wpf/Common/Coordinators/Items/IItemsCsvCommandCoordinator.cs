using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using GameTools.Client.Application.UseCases.Items.BulkInsertItems;
using GameTools.Client.Application.UseCases.Items.BulkUpdateItems;
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
        Task ImportAndBulkInsertAsync(CancellationToken external = default);
        Task ImportAndBulkUpdateAsync(CancellationToken external = default);
    }
}
