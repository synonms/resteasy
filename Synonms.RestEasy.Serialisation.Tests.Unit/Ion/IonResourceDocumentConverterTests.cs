using System.Text.Json;
using Synonms.RestEasy.Abstractions.Schema.Documents;
using Synonms.RestEasy.Abstractions.Schema.Errors;
using Synonms.RestEasy.Abstractions.Schema.Forms;
using Synonms.RestEasy.Serialisation.Ion;
using Synonms.RestEasy.Serialisation.Tests.Unit.Framework;
using Synonms.RestEasy.Serialisation.Tests.Unit.Framework.Assertions;

namespace Synonms.RestEasy.Serialisation.Tests.Unit.Ion;

public class IonResourceDocumentConverterTests
{
    private readonly JsonSerializerOptions _jsonSerialiserOptions;

    public IonResourceDocumentConverterTests()
    {
        _jsonSerialiserOptions = JsonSerializerOptionsFactory.CreateIon();
    }

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
        string json = JsonFactory.CreateResourceDocument(id, childId, otherId);
            
        ResourceDocument<TestResource>? resourceDocument = JsonSerializer.Deserialize<ResourceDocument<TestResource>>(json, _jsonSerialiserOptions);

        Assert.NotNull(resourceDocument);
        ResourceDocumentAssertions.Verify(resourceDocument!, id, childId, otherId);
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
        ResourceDocumentAssertions.Verify(jsonDocument.RootElement, id, childId, otherId);
    }
}