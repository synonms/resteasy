using Synonms.RestEasy.Abstractions.Schema;

namespace Synonms.RestEasy.Serialisation.Tests.Unit.Framework;

public static class ResourceFactory
{
    public const bool SomeBool = true;
    public const int SomeInt = 123;
    public const string SomeString = "test";
    
    public static TestResource Create(Guid id)
    {
        Link selfLink = Link.SelfLink(new Uri($"http://localhost:5000/resources/{id}"));
        Link editFormLink = Link.EditFormLink(new Uri($"http://localhost:5000/resources/{id}/edit-form"));
        Link deleteLink = Link.DeleteSelfLink(new Uri($"http://localhost:5000/resources/{id}"));

        TestResource resource = new(id, selfLink)
        {
            SomeBool = true,
            SomeInt = 123,
            SomeString = "test",
            SomeOptionalString = null
        };
        
        resource.Links.Add("edit-form", editFormLink);
        resource.Links.Add("delete", deleteLink);

        return resource;
    }
}