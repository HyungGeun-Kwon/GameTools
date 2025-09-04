using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DotNetHelper.MsDiKit.Common;
using DotNetHelper.MsDiKit.DialogServices;
using DotNetHelper.MsDiKit.RegionServices;
using GameTools.Client.Application.Common.Paging;
using GameTools.Client.Application.UseCases.Items.GetItemsPage;
using GameTools.Client.Domain.Items;
using GameTools.Client.Wpf.Common.Names;
using GameTools.Client.Wpf.Common.State;
using GameTools.Client.Wpf.ViewModels.Items.Mappers;

namespace GameTools.Client.Wpf.ViewModels.Items
{
    public sealed partial class ItemHeaderViewModel(
        IDialogService dialogService,
        IRegionService regionService,
        IItemPageSearchState itemPageSearchState,
        GetItemsPageUseCase getItemsPageUseCase)
        : ObservableObject, IRegionViewModel
    {
        [RelayCommand]
        private void AddItem()
        {
            dialogService.ShowDialog(DialogViewNames.Item_EditDialog, null, ItemEditDialogClosed);
        }

        private async void ItemEditDialogClosed(IDialogResult? result)
        {
            if (result?.ButtonResult != ButtonResult.OK) return;

            try
            {
                if (itemPageSearchState.Results.Count == 0) return;

                // 가장 최근에 검색했던 방식으로 검색.
                var pagination = new Pagination(itemPageSearchState.PageNumber, itemPageSearchState.PageSize);
                var itemFilter = new ItemSearchFilter(itemPageSearchState.NameFilter, itemPageSearchState.RarityIdFilter);
                var getItemPageInput = new GetItemsPageInput(pagination, itemFilter);

                PagedOutput<Item> itemPageOutput = await getItemsPageUseCase.Handle(getItemPageInput, CancellationToken.None);

                itemPageSearchState.ReplacePageResults(itemPageOutput.ToPagedItemEditModel());
            }
            catch (OperationCanceledException) { }
        }

        public void OnRegionActivated(Parameters? _)
        {
            regionService.SetView(RegionNames.Item_SearchRegion, RegionViewNames.Item_SearchView);
        }

        public void OnRegionDeactivated()
        {
        }
    }
}
