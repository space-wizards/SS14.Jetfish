namespace SS14.Jetfish.Core.Repositories;

public interface IRecord<out TKey>
{
    public TKey Id { get; }
    public int Version { get; }
}