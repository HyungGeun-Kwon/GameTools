using GameTools.Application.Abstractions.Users;

namespace GameTools.Test.Utils
{
    public class TestCurrentUser : ICurrentUser
    {
        public string UserIdOrName => "test_user";
    }
}
