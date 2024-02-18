using Synonms.RestEasy.Core.Extensions;

namespace Synonms.RestEasy.Core.Schema.Resources;

public abstract class ChildResource
{
    protected ChildResource()
    {
        Id = Guid.NewGuid().ToComb();
    }
    
    protected ChildResource(Guid id)
    {
        Id = id;
    }

    public Guid Id { get; init; }
    
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; init; }
}