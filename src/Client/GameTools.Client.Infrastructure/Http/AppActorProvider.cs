using GameTools.Client.Application.Ports;

namespace GameTools.Client.Infrastructure.Http
{
    public sealed class AppActorProvider : IActorProvider
    {
        public string? GetActor()
        {
            var envUser = Environment.UserName;
            return string.IsNullOrWhiteSpace(envUser) ? "client" : envUser;
        }
    }
}
