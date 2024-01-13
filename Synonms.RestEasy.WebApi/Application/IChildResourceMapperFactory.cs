namespace Synonms.RestEasy.WebApi.Application;

public interface IChildResourceMapperFactory
{
    IChildResourceMapper? Create(Type aggregateMemberType, Type childResourceType);
}