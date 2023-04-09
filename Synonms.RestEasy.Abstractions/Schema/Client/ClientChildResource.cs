namespace Synonms.RestEasy.Abstractions.Schema.Client;

public abstract class ClientChildResource
{
    protected ClientChildResource()
    {
        Id = Guid.Empty;
    }
    
    protected ClientChildResource(Guid id)
    {
        Id = id;
    }

    public Guid Id { get; init; }
}