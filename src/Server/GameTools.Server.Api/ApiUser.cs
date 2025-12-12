using GameTools.Server.Application.Abstractions.Users;

namespace GameTools.Server.Api
{
    public sealed class ApiUser : ICurrentUser
    {
        public string UserIdOrName { get; private set; } = "api";


        public void Set(string user) => UserIdOrName = user;
    }
}
