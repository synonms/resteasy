using System.Text.Json;
using Synonms.RestEasy.Abstractions.Constants;
using Synonms.RestEasy.Abstractions.Schema.Documents;
using Synonms.RestEasy.Serialisation.Ion.Constants;

namespace Synonms.RestEasy.Serialisation.Tests.Unit.Framework.Assertions;

public static class ResourceCollectionDocumentAssertions
{
    public static void Verify(JsonElement resourceCollectionDocumentElement, Guid expectedId1, Guid expectedChildId1, Guid expectedOtherId1, Guid expectedId2, Guid expectedChildId2, Guid expectedOtherId2)
    {
        JsonElement valueArrayElement = resourceCollectionDocumentElement.GetProperty(IonPropertyNames.Value);

        Assert.Equal(JsonValueKind.Array, valueArrayElement.ValueKind);

        Assert.Collection(valueArrayElement.EnumerateArray(),
            x => ResourceAssertions.Verify(x, expectedId1, expectedChildId1, expectedOtherId1),
            x => ResourceAssertions.Verify(x, expectedId2, expectedChildId2, expectedOtherId2));

        JsonElement selfElement = resourceCollectionDocumentElement.GetProperty(IanaLinkRelations.Self);
        Assert.Equal("http://localhost:5000/resources", selfElement.GetProperty(IonPropertyNames.Links.Uri).GetString());
        Assert.Equal("GET", selfElement.GetProperty(IonPropertyNames.Links.Method).GetString());
        Assert.Equal("self", selfElement.GetProperty(IonPropertyNames.Links.Relation).GetString());
        
        PaginationAssertions.Verify(resourceCollectionDocumentElement);
    }

    public static void Verify(ResourceCollectionDocument<TestResource> resourceCollectionDocument, Guid expectedId1, Guid expectedChildId1, Guid expectedOtherId1, Guid expectedId2, Guid expectedChildId2, Guid expectedOtherId2)
    {
        Assert.Collection(resourceCollectionDocument.Resources,
            x => ResourceAssertions.Verify(x, expectedId1, expectedChildId1, expectedOtherId1),
            x => ResourceAssertions.Verify(x, expectedId2, expectedChildId2, expectedOtherId2));

        Assert.True(resourceCollectionDocument.Links.ContainsKey(IanaLinkRelations.Self));
        Assert.Equal("http://localhost:5000/resources", resourceCollectionDocument.Links[IanaLinkRelations.Self].Uri.OriginalString);
        Assert.Equal("GET", resourceCollectionDocument.Links[IanaLinkRelations.Self].Method);
        Assert.Equal("self", resourceCollectionDocument.Links[IanaLinkRelations.Self].Relation);
        
        PaginationAssertions.Verify(resourceCollectionDocument.Pagination);
    }
}