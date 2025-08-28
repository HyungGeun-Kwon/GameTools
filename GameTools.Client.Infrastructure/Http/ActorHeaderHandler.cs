using GameTools.Client.Application.Ports;

namespace GameTools.Client.Infrastructure.Http
{
    /// <summary>
    /// X-Actor 헤더를 모든 요청에 추가.
    /// </summary>
    public sealed class ActorHeaderHandler(IActorProvider provider) : DelegatingHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var actor = provider.GetActor() ?? "console_user";
            if (request.Headers.Contains("X-Actor"))
                request.Headers.Remove("X-Actor");

            request.Headers.Add("X-Actor", actor);
            return base.SendAsync(request, cancellationToken);
        }
    }
}
