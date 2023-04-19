using System.Text.Json;
using Synonms.RestEasy.Abstractions.Domain;
using Synonms.RestEasy.Abstractions.Schema;
using Synonms.RestEasy.Serialisation.Ion;
using Synonms.RestEasy.Serialisation.Tests.Unit.Framework;

namespace Synonms.RestEasy.Serialisation.Tests.Unit.Ion;

public class IonServerResourceJsonConverterTests
{
    private readonly JsonSerializerOptions _jsonSerialiserOptions;

    public IonServerResourceJsonConverterTests()
    {
        _jsonSerialiserOptions = JsonSerializerOptionsFactory.CreateIon();
    }

    [Fact]
    public void CanConvert_IsSupported_ReturnsTrue()
    {
        IonServerResourceJsonConverter<TestAggregateRoot, TestServerResource> jsonConverter = new();
        
        Assert.True(jsonConverter.CanConvert(typeof(TestServerResource)));
    }
    
    [Theory]
    [InlineData(typeof(string))]
    [InlineData(typeof(Guid))]
    [InlineData(typeof(Form))]
    [InlineData(typeof(Error))]
    public void CanConvert_IsNotSupported_ReturnsFalse(Type typeToConvert)
    {
        IonServerResourceJsonConverter<TestAggregateRoot, TestServerResource> jsonConverter = new();
        
        Assert.False(jsonConverter.CanConvert(typeToConvert));
    }

    [Fact]
    public void Read_Valid_ReturnsResource()
    {
        EntityId<TestAggregateRoot> id = EntityId<TestAggregateRoot>.New();
        string json = ResourceFactory.CreateJson(id);
            
        TestServerResource? resource = JsonSerializer.Deserialize<TestServerResource>(json, _jsonSerialiserOptions);

        Assert.NotNull(resource);
        ResourceAssertions.Verify(resource!, id.Value);
    }

    [Fact]
    public void Write_Valid_ReturnsJson()
    {
        EntityId<TestAggregateRoot> id = EntityId<TestAggregateRoot>.New();
        TestServerResource resource = ResourceFactory.Create(id);

        string json = JsonSerializer.Serialize(resource, _jsonSerialiserOptions);

        JsonDocument jsonDocument = JsonDocument.Parse(json);
        ResourceAssertions.Verify(jsonDocument.RootElement, id.Value);
    }
}