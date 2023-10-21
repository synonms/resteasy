using Synonms.RestEasy.Abstractions.Schema;
using Synonms.RestEasy.Abstractions.Schema.Documents;

namespace Synonms.RestEasy.Serialisation.Tests.Unit.Framework;

public static class ResourceDocumentFactory
{
    public static ResourceDocument<TestResource> Create(Guid id, Guid childId, Guid otherId) => 
        new (Link.SelfLink(new Uri($"http://localhost:5000/resources/{id}")), ResourceFactory.Create(id, childId, otherId));
}