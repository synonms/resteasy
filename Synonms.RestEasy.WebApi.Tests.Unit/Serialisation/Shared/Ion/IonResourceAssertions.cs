using System;
using System.Text.Json;
using Synonms.RestEasy.WebApi.Constants;
using Synonms.RestEasy.WebApi.Schema;
using Synonms.RestEasy.WebApi.Serialisation.Ion;
using Synonms.RestEasy.WebApi.Tests.Unit.Serialisation.Shared.Resources;
using Xunit;

namespace Synonms.RestEasy.WebApi.Tests.Unit.Serialisation.Shared.Ion;

public static class IonResourceAssertions
{
    public static void Verify(JsonElement resourceElement, Guid expectedId, Guid expectedChildId, Guid expectedOtherId)
    {
        Assert.Equal(expectedId, resourceElement.GetProperty("id").GetGuid());
        Assert.Equal(ResourceFactory.SomeBool, resourceElement.GetProperty("someBool").GetBoolean());
        Assert.Equal(ResourceFactory.SomeInt, resourceElement.GetProperty("someInt").GetInt32());
        Assert.Equal(ResourceFactory.SomeString, resourceElement.GetProperty("someString").GetString());

        JsonElement selfElement = resourceElement.GetProperty(IanaLinkRelations.Self);
        Assert.Equal("http://localhost:5000/resources/" + expectedId, selfElement.GetProperty(IonPropertyNames.Links.Uri).GetString());
        Assert.Equal("GET", selfElement.GetProperty(IonPropertyNames.Links.Method).GetString());
        Assert.Equal("self", selfElement.GetProperty(IonPropertyNames.Links.Relation).GetString());

        JsonElement editFormElement = resourceElement.GetProperty(IanaLinkRelations.Forms.Edit);
        Assert.Equal("http://localhost:5000/resources/" + expectedId + "/edit-form", editFormElement.GetProperty(IonPropertyNames.Links.Uri).GetString());
        Assert.Equal("GET", editFormElement.GetProperty(IonPropertyNames.Links.Method).GetString());
        Assert.Equal("edit-form", editFormElement.GetProperty(IonPropertyNames.Links.Relation).GetString());

        JsonElement deleteElement = resourceElement.GetProperty("delete");
        Assert.Equal("http://localhost:5000/resources/" + expectedId, deleteElement.GetProperty(IonPropertyNames.Links.Uri).GetString());
        Assert.Equal("DELETE", deleteElement.GetProperty(IonPropertyNames.Links.Method).GetString());
        Assert.Equal("self", deleteElement.GetProperty(IonPropertyNames.Links.Relation).GetString());

        JsonElement childElement = resourceElement.GetProperty("someChild");
        
        Assert.Equal(expectedChildId, childElement.GetProperty("id").GetGuid());
        Assert.Equal(ResourceFactory.SomeBool, childElement.GetProperty("someBool").GetBoolean());
        Assert.Equal(ResourceFactory.SomeInt, childElement.GetProperty("someInt").GetInt32());
        Assert.Equal(ResourceFactory.SomeString, childElement.GetProperty("someString").GetString());

        JsonElement otherElement = resourceElement.GetProperty("someOther");
        
        Assert.Equal(expectedOtherId, otherElement.GetProperty("id").GetGuid());
        Assert.Equal(ResourceFactory.SomeBool, otherElement.GetProperty("someBool").GetBoolean());
        Assert.Equal(ResourceFactory.SomeInt, otherElement.GetProperty("someInt").GetInt32());
        Assert.Equal(ResourceFactory.SomeString, otherElement.GetProperty("someString").GetString());
        
        JsonElement otherSelfElement = otherElement.GetProperty(IanaLinkRelations.Self);
        Assert.Equal("http://localhost:5000/other-resources/" + expectedOtherId, otherSelfElement.GetProperty(IonPropertyNames.Links.Uri).GetString());
        Assert.Equal("GET", otherSelfElement.GetProperty(IonPropertyNames.Links.Method).GetString());
        Assert.Equal("self", otherSelfElement.GetProperty(IonPropertyNames.Links.Relation).GetString());

        JsonElement otherEditFormElement = otherElement.GetProperty(IanaLinkRelations.Forms.Edit);
        Assert.Equal("http://localhost:5000/other-resources/" + expectedOtherId + "/edit-form", otherEditFormElement.GetProperty(IonPropertyNames.Links.Uri).GetString());
        Assert.Equal("GET", otherEditFormElement.GetProperty(IonPropertyNames.Links.Method).GetString());
        Assert.Equal("edit-form", otherEditFormElement.GetProperty(IonPropertyNames.Links.Relation).GetString());

        JsonElement otherDeleteElement = otherElement.GetProperty("delete");
        Assert.Equal("http://localhost:5000/other-resources/" + expectedOtherId, otherDeleteElement.GetProperty(IonPropertyNames.Links.Uri).GetString());
        Assert.Equal("DELETE", otherDeleteElement.GetProperty(IonPropertyNames.Links.Method).GetString());
        Assert.Equal("self", otherDeleteElement.GetProperty(IonPropertyNames.Links.Relation).GetString());
    }

    public static void Verify(TestResource resource, Guid expectedId, Guid expectedChildId, Guid expectedOtherId)
    {
        Assert.Equal(expectedId, resource.Id);
        Assert.Equal(ResourceFactory.SomeBool, resource.SomeBool);
        Assert.Equal(ResourceFactory.SomeInt, resource.SomeInt);
        Assert.Equal(ResourceFactory.SomeString, resource.SomeString);

        Assert.Equal("http://localhost:5000/resources/" + resource.Id, resource.SelfLink.Uri.OriginalString);
        Assert.Equal("GET", resource.SelfLink.Method);
        Assert.Equal("self", resource.SelfLink.Relation);
        
        Assert.True(resource.Links.ContainsKey("edit-form"));
        Link editFormLink = resource.Links["edit-form"];
        Assert.Equal("http://localhost:5000/resources/" + resource.Id + "/edit-form", editFormLink.Uri.OriginalString);
        Assert.Equal("GET", editFormLink.Method);
        Assert.Equal("edit-form", editFormLink.Relation);
        
        Assert.True(resource.Links.ContainsKey("delete"));
        Link deleteLink = resource.Links["delete"];
        Assert.Equal("http://localhost:5000/resources/" + resource.Id, deleteLink.Uri.OriginalString);
        Assert.Equal("DELETE", deleteLink.Method);
        Assert.Equal("self", deleteLink.Relation);
        
        Assert.Equal(expectedChildId, resource.SomeChild.Id);
        Assert.Equal(ResourceFactory.SomeBool, resource.SomeChild.SomeBool);
        Assert.Equal(ResourceFactory.SomeInt, resource.SomeChild.SomeInt);
        Assert.Equal(ResourceFactory.SomeString, resource.SomeChild.SomeString);

        Assert.Equal(expectedOtherId, resource.SomeOtherId);
        Assert.Equal(expectedOtherId, resource.SomeOther.Id);
        Assert.Equal(ResourceFactory.SomeBool, resource.SomeOther.SomeBool);
        Assert.Equal(ResourceFactory.SomeInt, resource.SomeOther.SomeInt);
        Assert.Equal(ResourceFactory.SomeString, resource.SomeOther.SomeString);

        Assert.Equal("http://localhost:5000/other-resources/" + resource.SomeOtherId, resource.SomeOther.SelfLink.Uri.OriginalString);
        Assert.Equal("GET", resource.SomeOther.SelfLink.Method);
        Assert.Equal("self", resource.SomeOther.SelfLink.Relation);
        
        Assert.True(resource.SomeOther.Links.ContainsKey("edit-form"));
        Link otherEditFormLink = resource.SomeOther.Links["edit-form"];
        Assert.Equal("http://localhost:5000/other-resources/" + resource.SomeOtherId + "/edit-form", otherEditFormLink.Uri.OriginalString);
        Assert.Equal("GET", otherEditFormLink.Method);
        Assert.Equal("edit-form", otherEditFormLink.Relation);
        
        Assert.True(resource.SomeOther.Links.ContainsKey("delete"));
        Link otherDeleteLink = resource.SomeOther.Links["delete"];
        Assert.Equal("http://localhost:5000/other-resources/" + resource.SomeOtherId, otherDeleteLink.Uri.OriginalString);
        Assert.Equal("DELETE", otherDeleteLink.Method);
        Assert.Equal("self", otherDeleteLink.Relation);
    }
}