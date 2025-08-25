using GameTools.Client.Application.Ports;

namespace GameTools.Client.Application.UseCases.Rarities.DeleteRarity
{
    public sealed class DeleteRarityUseCase(IRarityGateway gateway)
    {
        public Task Handle(DeleteRarityInput input, CancellationToken ct)
            => gateway.DeleteAsync(input, ct);
    }
}