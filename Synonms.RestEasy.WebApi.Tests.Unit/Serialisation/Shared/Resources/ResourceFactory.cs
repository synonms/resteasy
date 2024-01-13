using System;
using Synonms.RestEasy.WebApi.Schema;

namespace Synonms.RestEasy.WebApi.Tests.Unit.Serialisation.Shared.Resources;

public static class ResourceFactory
{
    public const bool SomeBool = true;
    public const int SomeInt = 123;
    public const string SomeString = "test";
    
    public static TestResource Create(Guid id, Guid childId, Guid otherId)
    {
        Link selfLink = Link.SelfLink(new Uri($"http://localhost:5000/resources/{id}"));
        Link editFormLink = Link.EditFormLink(new Uri($"http://localhost:5000/resources/{id}/edit-form"));
        Link deleteLink = Link.DeleteSelfLink(new Uri($"http://localhost:5000/resources/{id}"));

        TestResource resource = new(id, selfLink)
        {
            SomeBool = SomeBool,
            SomeInt = SomeInt,
            SomeString = SomeString,
            SomeOptionalString = null,
            SomeChild = CreateChild(childId),
            SomeOtherId = otherId,
            SomeOther = CreateOther(otherId)
        };
        
        resource.Links.Add("edit-form", editFormLink);
        resource.Links.Add("delete", deleteLink);

        return resource;
    }
    
    public static OtherTestResource CreateOther(Guid id)
    {
        Link selfLink = Link.SelfLink(new Uri($"http://localhost:5000/other-resources/{id}"));
        Link editFormLink = Link.EditFormLink(new Uri($"http://localhost:5000/other-resources/{id}/edit-form"));
        Link deleteLink = Link.DeleteSelfLink(new Uri($"http://localhost:5000/other-resources/{id}"));

        OtherTestResource resource = new(id, selfLink)
        {
            SomeBool = SomeBool,
            SomeInt = SomeInt,
            SomeString = SomeString,
            SomeOptionalString = null
        };
        
        resource.Links.Add("edit-form", editFormLink);
        resource.Links.Add("delete", deleteLink);

        return resource;
    }
    
    public static TestChildResource CreateChild(Guid id)
    {
        TestChildResource resource = new(id)
        {
            SomeBool = SomeBool,
            SomeInt = SomeInt,
            SomeString = SomeString,
            SomeOptionalString = null
        };
        
        return resource;
    }
}