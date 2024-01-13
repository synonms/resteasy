using System.Collections.Generic;
using System.Linq;
using Synonms.RestEasy.WebApi.Domain;
using Synonms.RestEasy.WebApi.Runtime;
using Synonms.RestEasy.WebApi.Schema.Forms;
using Synonms.RestEasy.WebApi.Tests.Unit.Shared;
using NSubstitute;
using Xunit;

namespace Synonms.RestEasy.WebApi.Tests.Unit.Runtime;

public class ObjectExtensionsTests
{
    private readonly ILookupOptionsProvider _mockLookupOptionsProvider = Substitute.For<ILookupOptionsProvider>();
    
    public ObjectExtensionsTests()
    {
        _mockLookupOptionsProvider
            .Get(Arg.Any<string>())
            .Returns(Enumerable.Empty<FormFieldOption>());
    }

    [Fact]
    public void GetFormFields_ReturnsFormFields()
    {
        TestResource resource = new();
        
        List<FormField> formFields = resource.GetFormFields(_mockLookupOptionsProvider).ToList();

        Assert.Equal(10, formFields.Count());
    }
}