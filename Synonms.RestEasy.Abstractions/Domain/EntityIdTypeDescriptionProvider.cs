using System.ComponentModel;

namespace Synonms.RestEasy.Abstractions.Domain;

public class EntityIdTypeDescriptionProvider : TypeDescriptionProvider
{
    public override ICustomTypeDescriptor GetTypeDescriptor(Type objectType, object? instance)
    {
        return new EntityIdTypeDescriptor(objectType);
    }
}