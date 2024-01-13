using System.Net;
using Synonms.RestEasy.Core.Domain;
using Xunit;

namespace Synonms.RestEasy.Testing.Tests;

public abstract partial class AuthenticatedTestFixture<TWebApplicationFactory, TEntryPoint, TAggregateRoot, TResource>
{
    [Fact]
    public void Put_KnownId_Returns204NoContent() =>
        PutTest<TAggregateRoot, TResource>.Create(WebApplicationFactory.CreateClient(), CollectionPath, JsonSerialiserOptions, MediaType, PersistAggregateAsync, RetrieveAggregateAsync, ValidateAggregate)
            .Arrange
                .WithAggregate(GenerateUniqueAggregate())
                .WithResource(GenerateValidResource)
            .Act.WithAuthenticatedUser(UserId)
            .Assert.SucceedsWith(HttpStatusCode.NoContent);
    
    [Fact]
    public void Put_UnknownId_Returns404NotFound() =>
        PutTest<TAggregateRoot, TResource>.Create(WebApplicationFactory.CreateClient(), CollectionPath, JsonSerialiserOptions, MediaType, PersistAggregateAsync, RetrieveAggregateAsync, ValidateAggregate)
            .Arrange
                .WithoutAggregate()
                .WithResource(GenerateValidResource)
            .Act
                .WithAuthenticatedUser(UserId)
                .WithId(EntityId<TAggregateRoot>.New())
            .Assert.FailsWith(HttpStatusCode.NotFound);

    [Fact]
    public void Put_WithoutUser_Returns401Unauthorized() =>
        PutTest<TAggregateRoot, TResource>.Create(WebApplicationFactory.CreateClient(), CollectionPath, JsonSerialiserOptions, MediaType, PersistAggregateAsync, RetrieveAggregateAsync, ValidateAggregate)
            .Arrange
                .WithAggregate(GenerateUniqueAggregate())
                .WithResource(GenerateValidResource)
            .Act
            .Assert.FailsWith(HttpStatusCode.Unauthorized);
}