namespace GameTools.Server.Application.Abstractions.Users
{
    public interface ICurrentUser
    {
        string UserIdOrName { get; }
    }
}
