namespace GameTools.Server.Domain.Common
{
    public abstract class Entity<TKey>
    {
        public TKey Id { get; protected set; } = default!;
    }
}
