using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using GameTools.Client.Application.Common.Paging;
using GameTools.Client.Application.Ports;
using GameTools.Client.Application.UseCases.Items.BulkDeleteItems;
using GameTools.Client.Application.UseCases.Items.BulkInsertItems;
using GameTools.Client.Application.UseCases.Items.BulkUpdateItems;
using GameTools.Client.Application.UseCases.Items.CreateItem;
using GameTools.Client.Application.UseCases.Items.DeleteItem;
using GameTools.Client.Application.UseCases.Items.GetItemsPage;
using GameTools.Client.Application.UseCases.Items.RestoreItemsAsOf;
using GameTools.Client.Application.UseCases.Items.UpdateItem;
using GameTools.Client.Domain.Items;
using GameTools.Client.Infrastructure.Mapper;
using GameTools.Contracts.Common;
using GameTools.Contracts.Items.BulkDeleteItems;
using GameTools.Contracts.Items.BulkInsertItems;
using GameTools.Contracts.Items.BulkUpdateItems;
using GameTools.Contracts.Items.Common;
using GameTools.Contracts.Items.CreateItem;
using GameTools.Contracts.Items.GetItemPage;
using GameTools.Contracts.Items.GetItemsByRarity;
using GameTools.Contracts.Items.RestoreItemsAsOf;
using GameTools.Contracts.Items.UpdateItem;

namespace GameTools.Client.Infrastructure.Http
{
    public sealed class ItemsGateway(HttpClient http) : IItemsGateway
    {
        public async Task<Item?> GetByIdAsync(int id, CancellationToken ct)
        {
            var res = await http.GetAsync($"/api/items/{id}", ct);
            if (res.StatusCode == HttpStatusCode.NotFound) return null;

            res.EnsureSuccessStatusCode();
            ItemResponse? body = await res.Content.ReadFromJsonAsync<ItemResponse>(cancellationToken: ct);
            return body?.ToClient();
        }

        public async Task<IReadOnlyList<Item>> GetByRarityAsync(byte rarityId, CancellationToken ct)
        {
            var resp = await http.GetFromJsonAsync<ItemsByRarityResponse>($"/api/items/by-rarity/{rarityId}", ct)
               ?? new ItemsByRarityResponse([]);

            return resp.ToClient();
        }

        public async Task<PagedOutput<Item>> GetPageAsync(GetItemsPageInput input, CancellationToken ct)
        {
            ItemsPageRequest req = input.ToContract();
            using var res = await http.PostAsJsonAsync("/api/items/page-search", req, ct);
            res.EnsureSuccessStatusCode();

            var page = await res.Content.ReadFromJsonAsync<PagedResponse<ItemResponse>>(cancellationToken: ct)
                       ?? new PagedResponse<ItemResponse>([], 0, req.PageNumber, req.PageSize);

            return page.ToClient();
        }

        public async Task<Item> CreateAsync(CreateItemInput input, CancellationToken ct)
        {
            CreateItemRequest req = input.ToContract();
            using var res = await http.PostAsJsonAsync("/api/items", req, ct);
            res.EnsureSuccessStatusCode();

            var body = await res.Content.ReadFromJsonAsync<ItemResponse>(cancellationToken: ct)
                ?? throw new InvalidOperationException("Empty response");
            
            return body.ToClient();
        }

        public async Task<Item> UpdateAsync(UpdateItemInput input, CancellationToken ct)
        {
            UpdateItemRequest req = input.ToContract();
            using var res = await http.PutAsJsonAsync("/api/items", req, ct);
            res.EnsureSuccessStatusCode();

            var body = await res.Content.ReadFromJsonAsync<ItemResponse>(cancellationToken: ct)
                ?? throw new InvalidOperationException("Empty response");

            return body.ToClient();
        }

        public async Task DeleteAsync(DeleteItemInput input, CancellationToken ct)
        {
            using var req = new HttpRequestMessage(HttpMethod.Delete, $"/api/items/{input.Id}");

            req.Headers.IfMatch.Add(new EntityTagHeaderValue($"\"{input.RowVersionBase64}\""));

            using var res = await http.SendAsync(req, ct);

            res.EnsureSuccessStatusCode();
        }

        public async Task<BulkInsertItemsOutput> BulkInsertAsync(BulkInsertItemsInput input, CancellationToken ct)
        {
            BulkInsertItemsRequest req = input.ToContract();
            using var res = await http.PostAsJsonAsync("/api/items/bulk-insert", req, ct);
            res.EnsureSuccessStatusCode();

            var body = await res.Content.ReadFromJsonAsync<BulkInsertItemsResponse>(cancellationToken: ct)
                ?? throw new InvalidOperationException("Empty response");

            return body.ToClient();
        }

        public async Task<BulkUpdateItemsOutput> BulkUpdateAsync(BulkUpdateItemsInput input, CancellationToken ct)
        {
            BulkUpdateItemsRequest req = input.ToContract();
            using var res = await http.PutAsJsonAsync("/api/items/bulk-update", req, ct);
            res.EnsureSuccessStatusCode();

            var body = await res.Content.ReadFromJsonAsync<BulkUpdateItemsResponse>(cancellationToken: ct)
                ?? throw new InvalidOperationException("Empty response");

            return body.ToClient();
        }

        public async Task<BulkDeleteItemsOutput> BulkDeleteAsync(BulkDeleteItemsInput input, CancellationToken ct)
        {
            BulkDeleteItemsRequest req = input.ToContract();
            using var res = await http.PutAsJsonAsync("/api/items/bulk-delete", req, ct);
            res.EnsureSuccessStatusCode();

            var body = await res.Content.ReadFromJsonAsync<BulkDeleteItemsResponse>(cancellationToken: ct)
                ?? throw new InvalidOperationException("Empty response");

            return body.ToClient();
        }

        public async Task<RestoreItemsAsOfOutput> RestoreAsOfAsync(RestoreItemsAsOfInput input, CancellationToken ct)
        {
            RestoreItemsAsOfRequest req = input.ToContract();

            using var res = await http.PostAsJsonAsync("/api/items/restore-asof", req, ct);
            res.EnsureSuccessStatusCode();

            var body = await res.Content.ReadFromJsonAsync<RestoreItemsAsOfResponse>(cancellationToken: ct)
                ?? throw new InvalidOperationException("Empty response");

            return body.ToClient();
        }
    }
}
