using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GameTools.Client.Application.UseCases.Items.BulkDeleteItems;
using GameTools.Client.Application.UseCases.Items.BulkInsertItems;
using GameTools.Client.Application.UseCases.Items.BulkUpdateItems;
using GameTools.Client.Application.UseCases.Items.CreateItem;
using GameTools.Client.Application.UseCases.Items.DeleteItem;
using GameTools.Client.Application.UseCases.Items.GetItemsPage;
using GameTools.Client.Application.UseCases.Items.RestoreItemsAsOf;
using GameTools.Client.Application.UseCases.Items.UpdateItem;
using GameTools.Client.Domain.Items;
using GameTools.Client.Wpf.Common.State;
using GameTools.Client.Wpf.Models.Items;
using GameTools.Client.Wpf.ViewModels.Items.Contracts;
using GameTools.Client.Wpf.ViewModels.Items.Mappers;

namespace GameTools.Client.Wpf.Common.Coordinators.Items
{
    public sealed partial class ItemsCommandCoordinator(
        IItemPageSearchState itemPageSearchState,
        UpdateItemUseCase updateItemUseCase,
        DeleteItemUseCase deleteItemUseCase,
        CreateItemUseCase createItemUseCase,
        BulkInsertItemsUseCase bulkInsertItemsUseCase,
        BulkUpdateItemsUseCase bulkUpdateItemsUseCase,
        BulkDeleteItemsUseCase bulkDeleteItemsUseCase,
        RestoreItemsAsOfUseCase restoreItemsAsOfUseCase
        ) : CoordinatorBase(
            busyNotifier: itemPageSearchState.BusyState,
            busyPropertyName: nameof(itemPageSearchState.BusyState.CommandBusy),
            isBusy: () => itemPageSearchState.BusyState.CommandBusy,
            setBusy: v => itemPageSearchState.BusyState.CommandBusy = v
        ), IItemsCommandCoordinator
    {
        public Task<Item> CreateAsync(ItemCreateModel itemCreateModel, CancellationToken external = default)
            => RunExclusiveAsync(ct => createItemUseCase.Handle(itemCreateModel.ToCreateItemInput(), ct), external);

        public Task DeleteAsync(ItemEditModel itemEditModel, CancellationToken external = default)
            => RunExclusiveAsync(ct => deleteItemUseCase.Handle(itemEditModel.ToDeleteItemInput(), ct), external);

        public Task<Item> UpdateAsync(ItemEditModel itemEditModel, CancellationToken external = default)
            => RunExclusiveAsync(ct => updateItemUseCase.Handle(itemEditModel.ToUpdateItemInput(), ct), external);

        public Task<BulkInsertItemsOutput> BulkInsertAsync(IEnumerable<ItemEditModel> items, CancellationToken external = default)
            => RunExclusiveAsync(ct => bulkInsertItemsUseCase.Handle(items.ToBulkInsertItemsInput(), ct), external);

        public Task<BulkUpdateItemsOutput> BulkUpdateAsync(IEnumerable<ItemEditModel> items, CancellationToken external = default)
            => RunExclusiveAsync(ct => bulkUpdateItemsUseCase.Handle(items.ToBulkUpdateItemsInput(), ct), external);

        public Task<BulkDeleteItemsOutput> BulkDeleteAsync(IEnumerable<ItemEditModel> items, CancellationToken external = default)
            => RunExclusiveAsync(ct => bulkDeleteItemsUseCase.Handle(items.ToBulkDeleteItemsInput(), ct), external);

        public Task<RestoreItemsAsOfOutput> RestoreAsOfAsync(RestoreItemsAsOfInput restoreItemsAsOfInput, CancellationToken external = default)
            => RunExclusiveAsync(ct => restoreItemsAsOfUseCase.Handle(restoreItemsAsOfInput, ct), external);
    }
}
