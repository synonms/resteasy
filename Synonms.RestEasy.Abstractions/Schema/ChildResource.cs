using Synonms.RestEasy.SharedKernel.Extensions;

namespace Synonms.RestEasy.Abstractions.Schema;

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
}