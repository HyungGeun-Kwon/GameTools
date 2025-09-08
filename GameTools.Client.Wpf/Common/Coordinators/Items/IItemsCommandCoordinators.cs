using CommunityToolkit.Mvvm.Input;
using GameTools.Client.Application.UseCases.Items.BulkInsertItems;
using GameTools.Client.Application.UseCases.Items.BulkUpdateItems;
using GameTools.Client.Domain.Items;
using GameTools.Client.Wpf.Models.Items;
using GameTools.Client.Wpf.ViewModels.Items.Contracts;

namespace GameTools.Client.Wpf.Common.Coordinators.Items
{
    public interface IItemsCommandCoordinator : IDisposable
    {
        /// <summary>
        /// 진행 중인 Command 취소
        /// </summary>
        IRelayCommand CancelCommand { get; }

        Task<Item> UpdateAsync(ItemEditModel itemEditModel, bool throwCancelException = false, CancellationToken external = default);

        Task DeleteAsync(ItemEditModel itemEditModel, bool throwCancelException = false, CancellationToken external = default);

        Task<Item> CreateAsync(ItemCreateModel itemCreateModel, bool throwCancelException = false, CancellationToken external = default);

        Task<BulkInsertItemsOutput> BulkInsertAsync(BulkInsertItemsInput bulkInsertItemsInput, bool throwCancelException = false, CancellationToken external = default);
        Task<BulkUpdateItemsOutput> BulkUpdateAsync(BulkUpdateItemsInput bulkUpdateItemsInput, bool throwCancelException = false, CancellationToken external = default);
    }
}
