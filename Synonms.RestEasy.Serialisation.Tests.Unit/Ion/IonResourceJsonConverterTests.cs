using System.Text.Json;
using Synonms.RestEasy.Abstractions.Schema.Errors;
using Synonms.RestEasy.Abstractions.Schema.Forms;
using Synonms.RestEasy.Serialisation.Ion;
using Synonms.RestEasy.Serialisation.Tests.Unit.Framework;
using Synonms.RestEasy.Serialisation.Tests.Unit.Framework.Assertions;

namespace Synonms.RestEasy.Serialisation.Tests.Unit.Ion;

public class IonResourceJsonConverterTests
{
    private readonly JsonSerializerOptions _jsonSerialiserOptions;

    public IonResourceJsonConverterTests()
    {
        _jsonSerialiserOptions = JsonSerializerOptionsFactory.CreateIon();
    }

    [Fact]
    public void CanConvert_IsSupported_ReturnsTrue()
    {
        IonResourceJsonConverter<TestResource> jsonConverter = new();
        
        Assert.True(jsonConverter.CanConvert(typeof(TestResource)));
    }
    
    [Theory]
    [InlineData(typeof(string))]
    [InlineData(typeof(Guid))]
    [InlineData(typeof(Form))]
    [InlineData(typeof(Error))]
    public void CanConvert_IsNotSupported_ReturnsFalse(Type typeToConvert)
    {
        IonResourceJsonConverter<TestResource> jsonConverter = new();
        
        Assert.False(jsonConverter.CanConvert(typeToConvert));
    }

    [Fact]
    public void Read_Valid_ReturnsResource()
    {
        Guid id = Guid.NewGuid();
        Guid childId = Guid.NewGuid();
        Guid otherId = Guid.NewGuid();
        string json = JsonFactory.CreateResource(id, childId, otherId);
            
        TestResource? resource = JsonSerializer.Deserialize<TestResource>(json, _jsonSerialiserOptions);

        Assert.NotNull(resource);
        ResourceAssertions.Verify(resource!, id, childId, otherId);
    }

    [Fact]
    public void Write_Valid_ReturnsJson()
    {
        Guid id = Guid.NewGuid();
        Guid childId = Guid.NewGuid();
        Guid otherId = Guid.NewGuid();
        TestResource resource = ResourceFactory.Create(id, childId, otherId);

        string json = JsonSerializer.Serialize(resource, _jsonSerialiserOptions);

        JsonDocument jsonDocument = JsonDocument.Parse(json);
        ResourceAssertions.Verify(jsonDocument.RootElement, id, childId, otherId);
    }
}