using System;
using Synonms.RestEasy.Core.Schema;
using Synonms.RestEasy.Core.Schema.Resources;

namespace Synonms.RestEasy.WebApi.Tests.Unit.Serialisation.Shared.Resources;

public static class ResourceDocumentFactory
{
    public static ResourceDocument<TestResource> Create(Guid id, Guid childId, Guid otherId) => 
        new (Link.SelfLink(new Uri($"http://localhost:5000/resources/{id}")), ResourceFactory.Create(id, childId, otherId));
}