using System.Text.Json;
using Synonms.RestEasy.Abstractions.Schema.Documents;
using Synonms.RestEasy.Abstractions.Schema.Errors;
using Synonms.RestEasy.Abstractions.Schema.Forms;
using Synonms.RestEasy.Serialisation.Ion;
using Synonms.RestEasy.Serialisation.Tests.Unit.Framework;
using Synonms.RestEasy.Serialisation.Tests.Unit.Framework.Assertions;

namespace Synonms.RestEasy.Serialisation.Tests.Unit.Ion;

public class IonResourceCollectionDocumentConverterTests
{
    private readonly JsonSerializerOptions _jsonSerialiserOptions;

    public IonResourceCollectionDocumentConverterTests()
    {
        _jsonSerialiserOptions = JsonSerializerOptionsFactory.CreateIon();
    }

    [Fact]
    public void CanConvert_IsSupported_ReturnsTrue()
    {
        IonResourceCollectionDocumentJsonConverter<TestResource> jsonConverter = new();
        
        Assert.True(jsonConverter.CanConvert(typeof(ResourceCollectionDocument<TestResource>)));
    }
    
    [Theory]
    [InlineData(typeof(string))]
    [InlineData(typeof(Guid))]
    [InlineData(typeof(Form))]
    [InlineData(typeof(Error))]
    [InlineData(typeof(TestResource))]
    public void CanConvert_IsNotSupported_ReturnsFalse(Type typeToConvert)
    {
        IonResourceCollectionDocumentJsonConverter<TestResource> jsonConverter = new();
        
        Assert.False(jsonConverter.CanConvert(typeToConvert));
    }

    [Fact]
    public void Read_Valid_ReturnsResource()
    {
        Guid id1 = Guid.NewGuid();
        Guid id2 = Guid.NewGuid();
        string json = JsonFactory.CreateResourceCollectionDocument(id1, id2);
            
        ResourceCollectionDocument<TestResource>? resourceCollectionDocument = JsonSerializer.Deserialize<ResourceCollectionDocument<TestResource>>(json, _jsonSerialiserOptions);

        Assert.NotNull(resourceCollectionDocument);
        ResourceCollectionDocumentAssertions.Verify(resourceCollectionDocument!, id1, id2);
    }

    [Fact]
    public void Write_Valid_ReturnsJson()
    {
        Guid id1 = Guid.NewGuid();
        Guid id2 = Guid.NewGuid();
        ResourceCollectionDocument<TestResource> resourceCollectionDocument = ResourceCollectionDocumentFactory.Create(id1, id2);

        string json = JsonSerializer.Serialize(resourceCollectionDocument, _jsonSerialiserOptions);

        JsonDocument jsonDocument = JsonDocument.Parse(json);
        ResourceCollectionDocumentAssertions.Verify(jsonDocument.RootElement, id1, id2);
    }
}