using System.Net;
using Synonms.RestEasy.Core.Domain;
using Xunit;

namespace Synonms.RestEasy.Testing.Tests;

public abstract partial class AnonymousTestFixture<TWebApplicationFactory, TEntryPoint, TAggregateRoot, TResource>
{
    [Fact]
    public void Put_KnownIdWithValidResource_Returns204NoContent() =>
        PutTest<TAggregateRoot, TResource>.Create(WebApplicationFactory.CreateClient(), CollectionPath, JsonSerialiserOptions, MediaType, PersistAggregateAsync, RetrieveAggregateAsync, ValidateAggregate)
            .Arrange
                .WithAggregate(GenerateUniqueAggregate())
                .WithResource(GenerateValidResource)
            .Act
            .Assert.SucceedsWith(HttpStatusCode.NoContent);

    [Fact]
    public void Put_KnownIdWithInvalidResource_Returns400BadRequest() =>
        PutTest<TAggregateRoot, TResource>.Create(WebApplicationFactory.CreateClient(), CollectionPath, JsonSerialiserOptions, MediaType, PersistAggregateAsync, RetrieveAggregateAsync, ValidateAggregate)
            .Arrange
                .WithAggregate(GenerateUniqueAggregate())
                .WithResource(GenerateInvalidResource)
            .Act
            .Assert.FailsWith(HttpStatusCode.BadRequest);

    [Fact]
    public void Put_UnknownId_Returns404NotFound() =>
        PutTest<TAggregateRoot, TResource>.Create(WebApplicationFactory.CreateClient(), CollectionPath, JsonSerialiserOptions, MediaType, PersistAggregateAsync, RetrieveAggregateAsync, ValidateAggregate)
            .Arrange
                .WithoutAggregate()
                .WithResource(GenerateValidResource)
            .Act.WithId(EntityId<TAggregateRoot>.New())
            .Assert.FailsWith(HttpStatusCode.NotFound);
}