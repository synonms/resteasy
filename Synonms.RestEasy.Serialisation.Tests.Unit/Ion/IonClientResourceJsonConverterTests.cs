using System.Text.Json;
using Synonms.RestEasy.Abstractions.Domain;
using Synonms.RestEasy.Abstractions.Schema;
using Synonms.RestEasy.Serialisation.Ion;
using Synonms.RestEasy.Serialisation.Tests.Unit.Framework;

namespace Synonms.RestEasy.Serialisation.Tests.Unit.Ion;

public class IonClientResourceJsonConverterTests
{
    private readonly JsonSerializerOptions _jsonSerialiserOptions;

    public IonClientResourceJsonConverterTests()
    {
        _jsonSerialiserOptions = JsonSerializerOptionsFactory.CreateIon();
    }

    [Fact]
    public void CanConvert_IsSupported_ReturnsTrue()
    {
        IonClientResourceJsonConverter<TestClientResource> jsonConverter = new();
        
        Assert.True(jsonConverter.CanConvert(typeof(TestClientResource)));
    }
    
    [Theory]
    [InlineData(typeof(string))]
    [InlineData(typeof(Guid))]
    [InlineData(typeof(Form))]
    [InlineData(typeof(Error))]
    public void CanConvert_IsNotSupported_ReturnsFalse(Type typeToConvert)
    {
        IonClientResourceJsonConverter<TestClientResource> jsonConverter = new();
        
        Assert.False(jsonConverter.CanConvert(typeToConvert));
    }

    [Fact]
    public void Read_Valid_ReturnsResource()
    {
        EntityId<TestAggregateRoot> id = EntityId<TestAggregateRoot>.New();
        string json = ResourceFactory.CreateJson(id);
            
        TestClientResource? resource = JsonSerializer.Deserialize<TestClientResource>(json, _jsonSerialiserOptions);

        Assert.NotNull(resource);
        ResourceAssertions.Verify(resource!, id.Value);
    }

    [Fact]
    public void Write_Valid_ReturnsJson()
    {
        Guid id = Guid.NewGuid();
        TestClientResource resource = ResourceFactory.Create(id);

        string json = JsonSerializer.Serialize(resource, _jsonSerialiserOptions);

        JsonDocument jsonDocument = JsonDocument.Parse(json);
        ResourceAssertions.Verify(jsonDocument.RootElement, id);
    }
}