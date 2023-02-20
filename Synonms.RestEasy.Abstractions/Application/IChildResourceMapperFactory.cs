namespace Synonms.RestEasy.Abstractions.Application;

public interface IChildResourceMapperFactory
{
    IChildResourceMapper? Create(Type aggregateMemberType, Type childResourceType);
}