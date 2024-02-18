using System;
using System.Text.Json;
using Synonms.RestEasy.Core.Constants;
using Synonms.RestEasy.Core.Schema.Resources;
using Synonms.RestEasy.Core.Serialisation.Ion;
using Synonms.RestEasy.WebApi.Tests.Unit.Serialisation.Shared.Resources;
using Xunit;

namespace Synonms.RestEasy.WebApi.Tests.Unit.Serialisation.Shared.Ion;

public static  class IonResourceDocumentAssertions
{
    public static void Verify(JsonElement resourceDocumentElement, Guid expectedId, Guid expectedChildId, Guid expectedOtherId)
    {
        IonResourceAssertions.Verify(resourceDocumentElement.GetProperty(IonPropertyNames.Value), expectedId, expectedChildId, expectedOtherId);
        
        JsonElement selfElement = resourceDocumentElement.GetProperty(IanaLinkRelations.Self);
        Assert.Equal("http://localhost:5000/resources/" + expectedId, selfElement.GetProperty(IonPropertyNames.Links.Uri).GetString());
        Assert.Equal("GET", selfElement.GetProperty(IonPropertyNames.Links.Method).GetString());
        Assert.Equal("self", selfElement.GetProperty(IonPropertyNames.Links.Relation).GetString());
    }

    public static void Verify(ResourceDocument<TestResource> resourceDocument, Guid expectedId, Guid expectedChildId, Guid expectedOtherId)
    {
        IonResourceAssertions.Verify(resourceDocument.Resource, expectedId, expectedChildId, expectedOtherId);

        Assert.True(resourceDocument.Links.ContainsKey(IanaLinkRelations.Self));
        Assert.Equal("http://localhost:5000/resources/" + expectedId, resourceDocument.Links[IanaLinkRelations.Self].Uri.OriginalString);
        Assert.Equal("GET", resourceDocument.Links[IanaLinkRelations.Self].Method);
        Assert.Equal("self", resourceDocument.Links[IanaLinkRelations.Self].Relation);
    }
}