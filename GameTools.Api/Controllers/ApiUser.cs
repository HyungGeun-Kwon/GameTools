using GameTools.Application.Abstractions.Users;

namespace GameTools.Api.Controllers
{
    public sealed class CurrentUser() : ICurrentUser
    {
        public string UserIdOrName => "system";
    }
}
