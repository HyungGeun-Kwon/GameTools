namespace GameTools.Application.Abstractions.Users
{
    public interface ICurrentUser
    {
        string UserIdOrName { get; }
    }
}
