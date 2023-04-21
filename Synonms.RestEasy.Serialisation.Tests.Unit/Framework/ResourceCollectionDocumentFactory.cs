using Synonms.RestEasy.Abstractions.Schema;
using Synonms.RestEasy.Abstractions.Schema.Documents;

namespace Synonms.RestEasy.Serialisation.Tests.Unit.Framework;

public static class ResourceCollectionDocumentFactory
{
    public static ResourceCollectionDocument<TestResource> Create(Guid id1, Guid id2) => 
        new (Link.SelfLink(new Uri($"http://localhost:5000/resources")), new []{ ResourceFactory.Create(id1), ResourceFactory.Create(id2) }, PaginationFactory.Create());
}