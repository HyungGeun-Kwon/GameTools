using GameTools.Server.Application.Abstractions.Users;

namespace GameTools.Server.Test.Utils
{
    public class TestCurrentUser : ICurrentUser
    {
        public string UserIdOrName => "test_user";
    }
}
