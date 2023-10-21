using Synonms.RestEasy.Abstractions.Schema;
using Synonms.RestEasy.Abstractions.Schema.Documents;

namespace Synonms.RestEasy.Serialisation.Tests.Unit.Framework;

public static class ResourceCollectionDocumentFactory
{
    public static ResourceCollectionDocument<TestResource> Create(Guid id1, Guid childId1, Guid otherId1, Guid id2, Guid childId2, Guid otherId2) => 
        new (Link.SelfLink(new Uri($"http://localhost:5000/resources")), new []{ ResourceFactory.Create(id1, childId1, otherId1), ResourceFactory.Create(id2, childId2, otherId2) }, PaginationFactory.Create());
}