using CommunityToolkit.Mvvm.Input;
using GameTools.Client.Application.UseCases.Items.BulkDeleteItems;
using GameTools.Client.Application.UseCases.Items.BulkInsertItems;
using GameTools.Client.Application.UseCases.Items.BulkUpdateItems;
using GameTools.Client.Application.UseCases.Items.RestoreItemsAsOf;
using GameTools.Client.Domain.Items;
using GameTools.Client.Wpf.Features.Items.Data.Models;

namespace GameTools.Client.Wpf.Features.Items.Data.Coordinators
{
    public interface IItemsCommandCoordinator : IDisposable
    {
        /// <summary>
        /// 진행 중인 Command 취소
        /// </summary>
        IRelayCommand CancelCommand { get; }

        Task<Item> UpdateAsync(ItemEditModel itemEditModel, CancellationToken external = default);

        Task DeleteAsync(ItemEditModel itemEditModel, CancellationToken external = default);

        Task<Item> CreateAsync(ItemCreateModel itemCreateModel, CancellationToken external = default);

        Task<BulkInsertItemsOutput> BulkInsertAsync(IEnumerable<ItemEditModel> items, CancellationToken external = default);
        Task<BulkUpdateItemsOutput> BulkUpdateAsync(IEnumerable<ItemEditModel> items, CancellationToken external = default);
        Task<BulkDeleteItemsOutput> BulkDeleteAsync(IEnumerable<ItemEditModel> items, CancellationToken external = default);

        Task<RestoreItemsAsOfOutput> RestoreAsOfAsync(RestoreItemsAsOfInput restoreItemsAsOfInput, CancellationToken external = default);
    }
}
