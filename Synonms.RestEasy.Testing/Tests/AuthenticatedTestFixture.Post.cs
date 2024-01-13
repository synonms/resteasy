using System.Net;
using Xunit;

namespace Synonms.RestEasy.Testing.Tests;

public abstract partial class AuthenticatedTestFixture<TWebApplicationFactory, TEntryPoint, TAggregateRoot, TResource>
{
    [Fact]
    public void Post_Invalid_Returns400BadRequest() =>
        PostTest<TAggregateRoot, TResource>.Create(WebApplicationFactory.CreateClient(), CollectionPath, JsonSerialiserOptions, MediaType, PersistPrerequisitesAsync, RetrieveAggregateAsync, RemoveAggregateAsync, ValidateAggregate)
            .Arrange.WithResource(GenerateInvalidResource)
            .Act.WithAuthenticatedUser(UserId)
            .Assert.FailsWith(HttpStatusCode.BadRequest);

    [Fact]
    public void Post_Valid_Returns200OkWithLocation() =>
        PostTest<TAggregateRoot, TResource>.Create(WebApplicationFactory.CreateClient(), CollectionPath, JsonSerialiserOptions, MediaType, PersistPrerequisitesAsync, RetrieveAggregateAsync, RemoveAggregateAsync, ValidateAggregate)
            .Arrange.WithResource(GenerateValidResource)
            .Act.WithAuthenticatedUser(UserId)
            .Assert.SucceedsWith(HttpStatusCode.Created);

    [Fact]
    public void Post_WithoutUser_Returns401Unauthorized() =>
        PostTest<TAggregateRoot, TResource>.Create(WebApplicationFactory.CreateClient(), CollectionPath, JsonSerialiserOptions, MediaType, PersistPrerequisitesAsync, RetrieveAggregateAsync, RemoveAggregateAsync, ValidateAggregate)
            .Arrange.WithResource(GenerateValidResource)
            .Act
            .Assert.FailsWith(HttpStatusCode.Unauthorized);
}