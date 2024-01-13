using System;
using System.Text.Json;
using Synonms.RestEasy.WebApi.Schema;
using Synonms.RestEasy.WebApi.Schema.Errors;
using Synonms.RestEasy.WebApi.Schema.Forms;
using Synonms.RestEasy.WebApi.Serialisation.Ion;
using Synonms.RestEasy.WebApi.Tests.Unit.Serialisation.Shared.Ion;
using Synonms.RestEasy.WebApi.Tests.Unit.Serialisation.Shared.Resources;
using Xunit;

namespace Synonms.RestEasy.WebApi.Tests.Unit.Serialisation.Ion;

public class IonPaginationJsonConverterTests
{
    private readonly JsonSerializerOptions _jsonSerialiserOptions = IonJsonSerializerOptionsFactory.CreateIon();

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
        string json = "{" + IonJsonFactory.CreatePagination() + "}";
            
        Pagination? pagination = JsonSerializer.Deserialize<Pagination>(json, _jsonSerialiserOptions);

        Assert.NotNull(pagination);
        IonPaginationAssertions.Verify(pagination);
    }

    [Fact]
    public void Write_Valid_ReturnsJson()
    {
        Pagination pagination = PaginationFactory.Create();

        string json = "{" + JsonSerializer.Serialize(pagination, _jsonSerialiserOptions) + "}";

        JsonDocument jsonDocument = JsonDocument.Parse(json);
        IonPaginationAssertions.Verify(jsonDocument.RootElement);
    }
}