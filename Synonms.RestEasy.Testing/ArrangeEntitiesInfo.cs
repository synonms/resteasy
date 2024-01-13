namespace Synonms.RestEasy.Testing;

public class ArrangeEntitiesInfo
{
    public ArrangeEntitiesInfo(params object[]? entities)
    {
        Entities = entities;
    }
    
    public object[]? Entities { get; }
}