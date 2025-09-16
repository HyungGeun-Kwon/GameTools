using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using GameTools.Client.Application.Ports;
using GameTools.Client.Application.UseCases.Rarities.CreateRarity;
using GameTools.Client.Application.UseCases.Rarities.DeleteRarity;
using GameTools.Client.Application.UseCases.Rarities.UpdateRarity;
using GameTools.Client.Domain.Rarities;
using GameTools.Client.Infrastructure.Mapper;
using GameTools.Contracts.Rarities.Common;
using GameTools.Contracts.Rarities.CreateRarity;
using GameTools.Contracts.Rarities.GetAllRarities;
using GameTools.Contracts.Rarities.UpdateRarity;

namespace GameTools.Client.Infrastructure.Http
{
    public sealed class RarityGateway(HttpClient http) : IRarityGateway
    {
        public async Task<Rarity?> GetByIdAsync(byte id, CancellationToken ct)
        {
            using var res = await http.GetAsync($"/api/rarities/{id}", ct);
            if (res.StatusCode == HttpStatusCode.NotFound) return null;

            res.EnsureSuccessStatusCode();
            var body = await res.Content.ReadFromJsonAsync<RarityResponse>(cancellationToken: ct);
            return body?.ToClient();
        }

        public async Task<IReadOnlyList<Rarity>> GetAllRaritiesAsync(CancellationToken ct)
        {
            using var res = await http.GetAsync($"/api/rarities/", ct);

            res.EnsureSuccessStatusCode();
            var resp = await res.Content.ReadFromJsonAsync<AllRarityResponse>(cancellationToken: ct)
                ?? new AllRarityResponse([]);
            return resp.ToClient();
        }

        public async Task<Rarity> CreateAsync(CreateRarityInput input, CancellationToken ct)
        {
            CreateRarityRequest req = input.ToContract();
            using var res = await http.PostAsJsonAsync("/api/rarities", req, ct);
            res.EnsureSuccessStatusCode();
            var body = await res.Content.ReadFromJsonAsync<RarityResponse>(cancellationToken: ct)
                ?? throw new InvalidOperationException("Empty response");

            return body.ToClient();
        }

        public async Task<Rarity> UpdateAsync(UpdateRarityInput input, CancellationToken ct)
        {
            UpdateRarityRequest req = input.ToContract();
            using var res = await http.PutAsJsonAsync("/api/rarities", req, ct);
            res.EnsureSuccessStatusCode();

            var body = await res.Content.ReadFromJsonAsync<RarityResponse>(cancellationToken: ct)
                ?? throw new InvalidOperationException("Empty response");

            return body.ToClient();
        }

        public async Task DeleteAsync(DeleteRarityInput input, CancellationToken ct)
        {
            using var req = new HttpRequestMessage(HttpMethod.Delete, $"/api/rarities/{input.Id}");

            req.Headers.IfMatch.Add(new EntityTagHeaderValue($"\"{input.RowVersionBase64}\""));

            using var res = await http.SendAsync(req, ct);

            res.EnsureSuccessStatusCode();
        }
    }
}
