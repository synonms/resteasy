namespace Synonms.RestEasy.Abstractions.Application;

public interface IResourceMapperFactory
{
    IResourceMapper? Create(Type aggregateRootType, Type resourceType);
}