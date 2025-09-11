using GameTools.Client.Application.Ports;

namespace GameTools.Client.Application.UseCases.Items.RestoreItemsAsOf
{
    public sealed class RestoreItemsAsOfUseCase(IItemsGateway gateway)
    {
        public Task<RestoreItemsAsOfOutput> Handle(RestoreItemsAsOfInput input, CancellationToken ct)
            => gateway.RestoreAsOfAsync(input, ct);
    }
}
