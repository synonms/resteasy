using System.Text.Json;
using Synonms.RestEasy.Abstractions.Schema;

namespace Synonms.RestEasy.Serialisation.Tests.Unit.Framework;

public static class ResourceAssertions
{
    public static void Verify(JsonElement resourceElement, Guid expectedId)
    {
        Assert.Equal(expectedId, resourceElement.GetProperty("id").GetGuid());
        Assert.Equal(ResourceFactory.SomeBool, resourceElement.GetProperty("someBool").GetBoolean());
        Assert.Equal(ResourceFactory.SomeInt, resourceElement.GetProperty("someInt").GetInt32());
        Assert.Equal(ResourceFactory.SomeString, resourceElement.GetProperty("someString").GetString());
    }

    public static void Verify(TestResource resource, Guid expectedId)
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
    }
}