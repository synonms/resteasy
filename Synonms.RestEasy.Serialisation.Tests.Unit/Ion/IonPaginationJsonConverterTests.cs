using System.Text.Json;
using Synonms.RestEasy.Abstractions.Schema;
using Synonms.RestEasy.Abstractions.Schema.Errors;
using Synonms.RestEasy.Abstractions.Schema.Forms;
using Synonms.RestEasy.Serialisation.Ion;
using Synonms.RestEasy.Serialisation.Tests.Unit.Framework;
using Synonms.RestEasy.Serialisation.Tests.Unit.Framework.Assertions;

namespace Synonms.RestEasy.Serialisation.Tests.Unit.Ion;

public class IonPaginationJsonConverterTests
{
    private readonly JsonSerializerOptions _jsonSerialiserOptions;

    public IonPaginationJsonConverterTests()
    {
        _jsonSerialiserOptions = JsonSerializerOptionsFactory.CreateIon();
    }

    [Fact]
    public void CanConvert_IsSupported_ReturnsTrue()
    {
        IonPaginationJsonConverter jsonConverter = new();
        
        Assert.True(jsonConverter.CanConvert(typeof(Pagination)));
    }
    
    [Theory]
    [InlineData(typeof(string))]
    [InlineData(typeof(Guid))]
    [InlineData(typeof(Form))]
    [InlineData(typeof(Error))]
    public void CanConvert_IsNotSupported_ReturnsFalse(Type typeToConvert)
    {
        IonPaginationJsonConverter jsonConverter = new();
        
        Assert.False(jsonConverter.CanConvert(typeToConvert));
    }

    [Fact]
    public void Read_Valid_ReturnsResource()
    {
        Guid id = Guid.NewGuid();
        string json = "{" + JsonFactory.CreatePagination() + "}";
            
        Pagination? pagination = JsonSerializer.Deserialize<Pagination>(json, _jsonSerialiserOptions);

        Assert.NotNull(pagination);
        PaginationAssertions.Verify(pagination!);
    }

    [Fact]
    public void Write_Valid_ReturnsJson()
    {
        Pagination pagination = PaginationFactory.Create();

        string json = "{" + JsonSerializer.Serialize(pagination, _jsonSerialiserOptions) + "}";

        JsonDocument jsonDocument = JsonDocument.Parse(json);
        PaginationAssertions.Verify(jsonDocument.RootElement);
    }
}