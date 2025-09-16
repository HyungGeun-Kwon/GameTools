using GameTools.Server.Application.Abstractions.Users;

namespace GameTools.Server.Api
{
    public sealed class ApiUser : ICurrentUser
    {
        public string UserIdOrName => "api";
    }
}
