using System;
using System.Text.Json;
using Synonms.RestEasy.Core.Schema.Errors;
using Synonms.RestEasy.Core.Schema.Forms;
using Synonms.RestEasy.Core.Schema.Resources;
using Synonms.RestEasy.Core.Serialisation.Ion;
using Synonms.RestEasy.WebApi.Tests.Unit.Serialisation.Shared.Ion;
using Synonms.RestEasy.WebApi.Tests.Unit.Serialisation.Shared.Resources;
using Xunit;

namespace Synonms.RestEasy.WebApi.Tests.Unit.Serialisation.Ion;

public class IonResourceDocumentJsonConverterTests
{
    private readonly JsonSerializerOptions _jsonSerialiserOptions = IonJsonSerializerOptionsFactory.CreateIon();

    [Fact]
    public void CanConvert_IsSupported_ReturnsTrue()
    {
        IonResourceDocumentJsonConverter<TestResource> jsonConverter = new();
        
        Assert.True(jsonConverter.CanConvert(typeof(ResourceDocument<TestResource>)));
    }
    
    [Theory]
    [InlineData(typeof(string))]
    [InlineData(typeof(Guid))]
    [InlineData(typeof(Form))]
    [InlineData(typeof(Error))]
    [InlineData(typeof(TestResource))]
    public void CanConvert_IsNotSupported_ReturnsFalse(Type typeToConvert)
    {
        IonResourceDocumentJsonConverter<TestResource> jsonConverter = new();
        
        Assert.False(jsonConverter.CanConvert(typeToConvert));
    }

    [Fact]
    public void Read_Valid_ReturnsResource()
    {
        Guid id = Guid.NewGuid();
        Guid childId = Guid.NewGuid();
        Guid otherId = Guid.NewGuid();
        string json = IonJsonFactory.CreateResourceDocument(id, childId, otherId);
            
        ResourceDocument<TestResource>? resourceDocument = JsonSerializer.Deserialize<ResourceDocument<TestResource>>(json, _jsonSerialiserOptions);

        Assert.NotNull(resourceDocument);
        IonResourceDocumentAssertions.Verify(resourceDocument!, id, childId, otherId);
    }

    [Fact]
    public void Write_Valid_ReturnsJson()
    {
        Guid id = Guid.NewGuid();
        Guid childId = Guid.NewGuid();
        Guid otherId = Guid.NewGuid();
        ResourceDocument<TestResource> resourceDocument = ResourceDocumentFactory.Create(id, childId, otherId);

        string json = JsonSerializer.Serialize(resourceDocument, _jsonSerialiserOptions);

        JsonDocument jsonDocument = JsonDocument.Parse(json);
        IonResourceDocumentAssertions.Verify(jsonDocument.RootElement, id, childId, otherId);
    }
}