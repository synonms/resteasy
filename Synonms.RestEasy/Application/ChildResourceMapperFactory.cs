using Synonms.RestEasy.Abstractions.Application;

namespace Synonms.RestEasy.Application;

public class ChildResourceMapperFactory : IChildResourceMapperFactory
{
    private readonly IEnumerable<IChildResourceMapper> _childResourceMappers;

    public ChildResourceMapperFactory(IEnumerable<IChildResourceMapper> childResourceMappers)
    {
        _childResourceMappers = childResourceMappers;
    }
    
    public IChildResourceMapper? Create(Type aggregateMemberType, Type childResourceType)
    {
        Type closedGenericMapperType = typeof(IChildResourceMapper<,>).MakeGenericType(aggregateMemberType, childResourceType);

        IChildResourceMapper? mapper = _childResourceMappers.FirstOrDefault(x => x.GetType().IsAssignableTo(closedGenericMapperType));

        return mapper;
    }
}