using System;
using System.Text.Json;
using Synonms.RestEasy.WebApi.Schema.Errors;
using Synonms.RestEasy.WebApi.Schema.Forms;
using Synonms.RestEasy.WebApi.Schema.Resources;
using Synonms.RestEasy.WebApi.Serialisation.Ion;
using Synonms.RestEasy.WebApi.Tests.Unit.Serialisation.Shared.Ion;
using Synonms.RestEasy.WebApi.Tests.Unit.Serialisation.Shared.Resources;
using Xunit;

namespace Synonms.RestEasy.WebApi.Tests.Unit.Serialisation.Ion;

public class IonResourceCollectionDocumentJsonConverterTests
{
    private readonly JsonSerializerOptions _jsonSerialiserOptions = IonJsonSerializerOptionsFactory.CreateIon();

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
        Guid childId1 = Guid.NewGuid();
        Guid otherId1 = Guid.NewGuid();
        Guid id2 = Guid.NewGuid();
        Guid childId2 = Guid.NewGuid();
        Guid otherId2 = Guid.NewGuid();
        string json = IonJsonFactory.CreateResourceCollectionDocument(id1, childId1, otherId1, id2, childId2, otherId2);
            
        ResourceCollectionDocument<TestResource>? resourceCollectionDocument = JsonSerializer.Deserialize<ResourceCollectionDocument<TestResource>>(json, _jsonSerialiserOptions);

        Assert.NotNull(resourceCollectionDocument);
        IonResourceCollectionDocumentAssertions.Verify(resourceCollectionDocument!, id1, childId1, otherId1, id2, childId2, otherId2);
    }

    [Fact]
    public void Write_Valid_ReturnsJson()
    {
        Guid id1 = Guid.NewGuid();
        Guid childId1 = Guid.NewGuid();
        Guid otherId1 = Guid.NewGuid();
        Guid id2 = Guid.NewGuid();
        Guid childId2 = Guid.NewGuid();
        Guid otherId2 = Guid.NewGuid();
        ResourceCollectionDocument<TestResource> resourceCollectionDocument = ResourceCollectionDocumentFactory.Create(id1, childId1, otherId1, id2, childId2, otherId2);

        string json = JsonSerializer.Serialize(resourceCollectionDocument, _jsonSerialiserOptions);

        JsonDocument jsonDocument = JsonDocument.Parse(json);
        IonResourceCollectionDocumentAssertions.Verify(jsonDocument.RootElement, id1, childId1, otherId1, id2, childId2, otherId2);
    }
}