using System.Text.Json;
using System.Text.Json.Serialization;
using Synonms.RestEasy.Testing.Tests;
using Synonms.RestEasy.Core.Domain;
using Synonms.RestEasy.Core.Serialisation;
using Synonms.RestEasy.WebApi.Constants;
using Synonms.RestEasy.WebApi.Schema.Resources;
using Synonms.RestEasy.WebApi.Serialisation.Ion;

namespace Synonms.RestEasy.Sample.Api.Tests.Integration.Infrastructure;

[Collection("SampleApiTestFixtures")]
public abstract class SampleApiAuthorisedTestFixture<TAggregateRoot, TResource> : AnonymousTestFixture<SampleWebApplicationFactory, Program, TAggregateRoot, TResource>
    where TAggregateRoot : AggregateRoot<TAggregateRoot>
    where TResource : Resource
{
    protected static readonly JsonSerializerOptions IonJsonSerializerOptions = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters =
        {
            new DateOnlyJsonConverter(),
            new OptionalDateOnlyJsonConverter(),
            new TimeOnlyJsonConverter(),
            new OptionalTimeOnlyJsonConverter(),
            new IonCustomJsonConverterFactory(),
            new IonLinkJsonConverter(),
            new IonFormDocumentJsonConverter(),
            new IonFormFieldJsonConverter(),
            new IonPaginationJsonConverter()
        }
    };
    
    protected SampleApiAuthorisedTestFixture(int pageLimit) 
        : base(new SampleWebApplicationFactory(), MediaTypes.Ion, IonJsonSerializerOptions, pageLimit)
    {
    }
}