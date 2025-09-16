namespace GameTools.Server.Application.Abstractions.Users
{
    public sealed class ApiCurrentUser : ICurrentUser
    {
        private string? _userIdOrName;
        public string UserIdOrName
        {
            get => _userIdOrName ?? "api_unknown";
            private set => _userIdOrName = value;
        }

        public void Set(string idOrName) => UserIdOrName = idOrName;
    }
}
