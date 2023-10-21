using System.Text.Json;
using Synonms.RestEasy.Abstractions.Constants;
using Synonms.RestEasy.Abstractions.Schema.Documents;
using Synonms.RestEasy.Serialisation.Ion.Constants;

namespace Synonms.RestEasy.Serialisation.Tests.Unit.Framework.Assertions;

public static class ResourceDocumentAssertions
{
    public static void Verify(JsonElement resourceDocumentElement, Guid expectedId, Guid expectedChildId, Guid expectedOtherId)
    {
        ResourceAssertions.Verify(resourceDocumentElement.GetProperty(IonPropertyNames.Value), expectedId, expectedChildId, expectedOtherId);
        
        JsonElement selfElement = resourceDocumentElement.GetProperty(IanaLinkRelations.Self);
        Assert.Equal("http://localhost:5000/resources/" + expectedId, selfElement.GetProperty(IonPropertyNames.Links.Uri).GetString());
        Assert.Equal("GET", selfElement.GetProperty(IonPropertyNames.Links.Method).GetString());
        Assert.Equal("self", selfElement.GetProperty(IonPropertyNames.Links.Relation).GetString());
    }

    public static void Verify(ResourceDocument<TestResource> resourceDocument, Guid expectedId, Guid expectedChildId, Guid expectedOtherId)
    {
        ResourceAssertions.Verify(resourceDocument.Resource, expectedId, expectedChildId, expectedOtherId);

        Assert.True(resourceDocument.Links.ContainsKey(IanaLinkRelations.Self));
        Assert.Equal("http://localhost:5000/resources/" + expectedId, resourceDocument.Links[IanaLinkRelations.Self].Uri.OriginalString);
        Assert.Equal("GET", resourceDocument.Links[IanaLinkRelations.Self].Method);
        Assert.Equal("self", resourceDocument.Links[IanaLinkRelations.Self].Relation);
    }
}