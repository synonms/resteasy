﻿using Microsoft.Extensions.DependencyInjection;
using Synonms.RestEasy.Abstractions.Application;

namespace Synonms.RestEasy.Application;

public class ChildResourceMapperFactory : IChildResourceMapperFactory
{
    private readonly IServiceProvider _serviceProvider;

    public ChildResourceMapperFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    
    public IChildResourceMapper? Create(Type aggregateMemberType, Type childResourceType)
    {
        IEnumerable<IChildResourceMapper> childResourceMappers = _serviceProvider.GetRequiredService<IEnumerable<IChildResourceMapper>>();
        
        Type closedGenericMapperType = typeof(IChildResourceMapper<,>).MakeGenericType(aggregateMemberType, childResourceType);

        IChildResourceMapper? mapper = childResourceMappers.FirstOrDefault(x => x.GetType().IsAssignableTo(closedGenericMapperType));

        return mapper;
    }
}