using GameTools.Client.Application.Common.Paging;
using GameTools.Client.Application.UseCases.Items.BulkInsertItems;
using GameTools.Client.Application.UseCases.Items.BulkUpdateItems;
using GameTools.Client.Application.UseCases.Items.CreateItem;
using GameTools.Client.Application.UseCases.Items.DeleteItem;
using GameTools.Client.Application.UseCases.Items.GetItemsPage;
using GameTools.Client.Application.UseCases.Items.UpdateItem;
using GameTools.Client.Domain.Items;
using GameTools.Client.Wpf.Common.State;
using GameTools.Client.Wpf.Models.Items;
using GameTools.Client.Wpf.ViewModels.Items.Contracts;

namespace GameTools.Client.Wpf.ViewModels.Items.Mappers
{
    public static class ItemMapper
    {
        public static ItemEditModel ToEditModel(this Item item)
            => new(item.Id, item.Name, item.Price, item.Description, item.RarityId, item.RowVersionBase64);
        
        public static Item ToDomain(this ItemEditModel itemEditModel, string grade, string colorCode)
        {
            if (!itemEditModel.Id.HasValue)
                throw new ArgumentException("Id must not be null.", nameof(itemEditModel));
            ArgumentException.ThrowIfNullOrWhiteSpace(itemEditModel.RowVersionBase64);

            return new(itemEditModel.Id.Value, itemEditModel.Name, itemEditModel.Price, itemEditModel.Description,
                itemEditModel.RarityId, grade, colorCode, itemEditModel.RowVersionBase64);
        }

        public static IEnumerable<ItemEditModel> ToEditModels(this IEnumerable<Item>? items)
            => (items ?? []).Select(i => i.ToEditModel());

        public static CreateItemInput ToCreateItemInput(this ItemCreateModel itemEditModel)
            => new(itemEditModel.Name, itemEditModel.Price, itemEditModel.RarityId, itemEditModel.Description);

        public static PagedOutput<ItemEditModel> ToPagedItemEditModel(this PagedOutput<Item> pagedOutput)
            => new(pagedOutput.Items.ToEditModels().ToList(), pagedOutput.TotalCount, pagedOutput.PageNumber, pagedOutput.PageSize);

        public static GetItemsPageInput ToGetItemPageInput(this IItemPageSearchState itemPageSearchState)
            => new
            (
                new(itemPageSearchState.PageNumber, itemPageSearchState.PageSize), 
                new(itemPageSearchState.NameFilter, itemPageSearchState.RarityIdFilter)
            );

        public static GetItemsPageInput GetItemPageInputFromNewPage(this IItemPageSearchState itemPageSearchState, int page)
            => new
            (
                new(page, itemPageSearchState.PageSize),
                new(itemPageSearchState.NameFilter, itemPageSearchState.RarityIdFilter)
            );

        public static CreateItemInput ToCreateItemInput(this ItemEditModel itemEditModel)
        {
            if (itemEditModel.Id is not null || itemEditModel.RowVersionBase64 is not null)
            {
                throw new InvalidOperationException("CreateItemInput requires Id or RowVersionBase64 to be null.");
            }
            return new(itemEditModel.Name, itemEditModel.Price, itemEditModel.RarityId, itemEditModel.Description);
        }

        public static DeleteItemInput ToDeleteItemInput(this ItemEditModel itemEditModel)
        {
            if (itemEditModel.Id is null)
            {
                throw new ArgumentNullException(nameof(itemEditModel), "DeleteItemInput requires a valid Id.");
            }
            if (string.IsNullOrEmpty(itemEditModel.RowVersionBase64))
            {
                throw new ArgumentNullException(nameof(itemEditModel), "DeleteItemInput requires a valid RowVersionBase64.");
            }
            return new(itemEditModel.Id.Value, itemEditModel.RowVersionBase64);
        }

        public static UpdateItemInput ToUpdateItemInput(this ItemEditModel itemEditModel)
        {
            if (itemEditModel.Id is null)
            {
                throw new ArgumentNullException(nameof(itemEditModel), "UpdateItemInput requires a valid Id.");
            }
            if (string.IsNullOrEmpty(itemEditModel.RowVersionBase64))
            {
                throw new ArgumentNullException(nameof(itemEditModel), "UpdateItemInput requires a valid RowVersionBase64.");
            }

            return new(
                itemEditModel.Id.Value, itemEditModel.Name, itemEditModel.Price, itemEditModel.Description,
                itemEditModel.RarityId, itemEditModel.RowVersionBase64);
        }

        public static BulkInsertItemInputRow ToBulkInsertItemInputRow(this ItemEditModel itemEditModel)
        {
            if (itemEditModel.Id is not null || itemEditModel.RowVersionBase64 is not null)
            {
                throw new InvalidOperationException("BulkInsertItemInputRow requires Id or RowVersionBase64 to be null.");
            }

            return new(itemEditModel.Name, itemEditModel.Price, itemEditModel.Description, itemEditModel.RarityId);
        }

        public static BulkInsertItemsInput ToBulkInsertItemsInput(this IEnumerable<ItemEditModel> itemEditModels)
            => new(itemEditModels.Select(im => im.ToBulkInsertItemInputRow()).ToList());

        public static BulkUpdateItemInputRow ToBulkUpdateItemInputRow(this ItemEditModel itemEditModel)
        {
            if (itemEditModel.Id is null)
            {
                throw new ArgumentNullException(nameof(itemEditModel), "BulkUpdateItemInputRow requires a valid Id.");
            }
            if (string.IsNullOrEmpty(itemEditModel.RowVersionBase64))
            {
                throw new ArgumentNullException(nameof(itemEditModel), "BulkUpdateItemInputRow requires a valid RowVersionBase64.");
            }

            return new(
                itemEditModel.Id.Value, itemEditModel.Name, itemEditModel.Price, itemEditModel.Description,
                itemEditModel.RarityId, itemEditModel.RowVersionBase64);
        }

        public static BulkUpdateItemsInput ToBulkUpdateItemsInput(this IEnumerable<ItemEditModel> itemEditModels)
            => new(itemEditModels.Select(im => im.ToBulkUpdateItemInputRow()).ToList());
    }
}
