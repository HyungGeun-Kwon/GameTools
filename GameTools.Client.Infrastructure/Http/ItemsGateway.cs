using System.Net.Http.Json;
using GameTools.Client.Application.Common.Paging;
using GameTools.Client.Application.Ports;
using GameTools.Client.Application.UseCases.BulkInsertItems;
using GameTools.Client.Application.UseCases.CreateItem;
using GameTools.Client.Application.UseCases.DeleteItem;
using GameTools.Client.Application.UseCases.GetItemsPage;
using GameTools.Client.Domain.Items;
using GameTools.Client.Infrastructure.Mapper;
using GameTools.Contracts.Common;
using GameTools.Contracts.Items.BulkInsertItems;
using GameTools.Contracts.Items.Common;
using GameTools.Contracts.Items.CreateItem;
using GameTools.Contracts.Items.DeleteItem;
using GameTools.Contracts.Items.GetItemPage;
using GameTools.Contracts.Items.GetItemsByRarity;
using GameTools.Contracts.Items.UpdateItem;

namespace GameTools.Client.Infrastructure.Http
{
    public sealed class ItemsGateway(HttpClient http) : IItemsGateway
    {
        public async Task<Item?> GetByIdAsync(int id, CancellationToken ct)
        {
            var res = await http.GetAsync($"/api/items/{id}", ct);
            if (res.StatusCode == System.Net.HttpStatusCode.NotFound) return null;

            res.EnsureSuccessStatusCode();
            ItemResponse? body = await res.Content.ReadFromJsonAsync<ItemResponse>(cancellationToken: ct);
            return body?.ToDomain();
        }

        public async Task<IReadOnlyList<Item>> GetByRarityAsync(byte rarityId, CancellationToken ct)
        {
            var resp = await http.GetFromJsonAsync<ItemsByRarityResponse>($"/api/items/by-rarity/{rarityId}", ct)
               ?? new ItemsByRarityResponse([]);

            return resp.ToDomain();
        }

        public async Task<PagedOutput<Item>> GetPageAsync(GetItemsPageInput input, CancellationToken ct)
        {
            ItemsPageRequest req = input.ToContract();
            using var res = await http.PostAsJsonAsync("/api/items/page-search", req, ct);
            res.EnsureSuccessStatusCode();

            var page = await res.Content.ReadFromJsonAsync<PagedResponse<ItemResponse>>(cancellationToken: ct)
                       ?? new PagedResponse<ItemResponse>([], 0, req.PageNumber, req.PageSize);

            return page.ToDomain();
        }

        public async Task<Item> CreateAsync(CreateItemInput input, CancellationToken ct)
        {
            var req = new CreateItemRequest(input.Name, input.Price, input.RarityId, input.Description);
            using var res = await http.PostAsJsonAsync("/api/items", req, ct);
            res.EnsureSuccessStatusCode();

            var body = await res.Content.ReadFromJsonAsync<ItemResponse>(cancellationToken: ct)
                ?? throw new InvalidOperationException("Empty response");

            return body.ToDomain();
        }

        public async Task<Item> UpdateAsync(UpdateItemInput input, CancellationToken ct)
        {
            var req = new UpdateItemRequest(input.Id, input.Name, input.Price, input.Description, input.RarityId, input.RowVersionBase64);
            using var res = await http.PutAsJsonAsync("/api/items", req, ct);
            res.EnsureSuccessStatusCode();

            var body = await res.Content.ReadFromJsonAsync<ItemResponse>(cancellationToken: ct)
                ?? throw new InvalidOperationException("Empty response");

            return body.ToDomain();
        }

        public async Task DeleteAsync(DeleteItemInput input, CancellationToken ct)
        {
            var reqBody = new DeleteItemRequest(input.Id, input.RowVersionBase64);
            using var req = new HttpRequestMessage(HttpMethod.Delete, "/api/items")
            {
                Content = JsonContent.Create(reqBody)
            };
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

            return body.ToDomain();
        }
    }
}
