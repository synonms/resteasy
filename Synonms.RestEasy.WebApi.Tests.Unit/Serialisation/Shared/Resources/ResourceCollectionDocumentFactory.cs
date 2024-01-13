using System;
using Synonms.RestEasy.WebApi.Schema;
using Synonms.RestEasy.WebApi.Schema.Resources;

namespace Synonms.RestEasy.WebApi.Tests.Unit.Serialisation.Shared.Resources;

public static class ResourceCollectionDocumentFactory
{
    public static ResourceCollectionDocument<TestResource> Create(Guid id1, Guid childId1, Guid otherId1, Guid id2, Guid childId2, Guid otherId2) => 
        new (Link.SelfLink(new Uri($"http://localhost:5000/resources")), new []{ ResourceFactory.Create(id1, childId1, otherId1), ResourceFactory.Create(id2, childId2, otherId2) }, PaginationFactory.Create());
}