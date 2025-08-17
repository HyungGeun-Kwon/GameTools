using GameTools.Application.Abstractions.Users;

namespace GameTools.Api
{
    public sealed class ApiUser : ICurrentUser
    {
        public string UserIdOrName => "api";
    }
}
