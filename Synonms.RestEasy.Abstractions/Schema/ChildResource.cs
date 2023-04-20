namespace Synonms.RestEasy.Abstractions.Schema;

public abstract class ChildResource
{
    protected ChildResource()
    {
        Id = Guid.Empty;
    }
    
    protected ChildResource(Guid id)
    {
        Id = id;
    }

    public Guid Id { get; init; }
}