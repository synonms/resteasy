using System.Text.Json;
using System.Text.Json.Serialization;
using Synonms.RestEasy.Core.Constants;
using Synonms.RestEasy.Testing.Tests;
using Synonms.RestEasy.Core.Domain;
using Synonms.RestEasy.Core.Schema.Resources;
using Synonms.RestEasy.Core.Serialisation;
using Synonms.RestEasy.Core.Serialisation.Ion;

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