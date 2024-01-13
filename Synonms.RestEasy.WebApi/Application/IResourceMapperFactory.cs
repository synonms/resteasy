namespace Synonms.RestEasy.WebApi.Application;

public interface IResourceMapperFactory
{
    IResourceMapper? Create(Type aggregateRootType, Type resourceType);
}