using System;
using System.Text.Json;
using Synonms.RestEasy.Core.Constants;
using Synonms.RestEasy.Core.Schema.Resources;
using Synonms.RestEasy.Core.Serialisation.Ion;
using Synonms.RestEasy.WebApi.Tests.Unit.Serialisation.Shared.Resources;
using Xunit;

namespace Synonms.RestEasy.WebApi.Tests.Unit.Serialisation.Shared.Ion;

public static class IonResourceCollectionDocumentAssertions
{
    public static void Verify(JsonElement resourceCollectionDocumentElement, Guid expectedId1, Guid expectedChildId1, Guid expectedOtherId1, Guid expectedId2, Guid expectedChildId2, Guid expectedOtherId2)
    {
        JsonElement valueArrayElement = resourceCollectionDocumentElement.GetProperty(IonPropertyNames.Value);

        Assert.Equal(JsonValueKind.Array, valueArrayElement.ValueKind);

        Assert.Collection(valueArrayElement.EnumerateArray(),
            x => IonResourceAssertions.Verify(x, expectedId1, expectedChildId1, expectedOtherId1),
            x => IonResourceAssertions.Verify(x, expectedId2, expectedChildId2, expectedOtherId2));

        JsonElement selfElement = resourceCollectionDocumentElement.GetProperty(IanaLinkRelations.Self);
        Assert.Equal("http://localhost:5000/resources", selfElement.GetProperty(IonPropertyNames.Links.Uri).GetString());
        Assert.Equal("GET", selfElement.GetProperty(IonPropertyNames.Links.Method).GetString());
        Assert.Equal("self", selfElement.GetProperty(IonPropertyNames.Links.Relation).GetString());
        
        IonPaginationAssertions.Verify(resourceCollectionDocumentElement);
    }

    public static void Verify(ResourceCollectionDocument<TestResource> resourceCollectionDocument, Guid expectedId1, Guid expectedChildId1, Guid expectedOtherId1, Guid expectedId2, Guid expectedChildId2, Guid expectedOtherId2)
    {
        Assert.Collection(resourceCollectionDocument.Resources,
            x => IonResourceAssertions.Verify(x, expectedId1, expectedChildId1, expectedOtherId1),
            x => IonResourceAssertions.Verify(x, expectedId2, expectedChildId2, expectedOtherId2));

        Assert.True(resourceCollectionDocument.Links.ContainsKey(IanaLinkRelations.Self));
        Assert.Equal("http://localhost:5000/resources", resourceCollectionDocument.Links[IanaLinkRelations.Self].Uri.OriginalString);
        Assert.Equal("GET", resourceCollectionDocument.Links[IanaLinkRelations.Self].Method);
        Assert.Equal("self", resourceCollectionDocument.Links[IanaLinkRelations.Self].Relation);
        
        IonPaginationAssertions.Verify(resourceCollectionDocument.Pagination);
    }
}