using System.Net;
using Synonms.RestEasy.Core.Domain;
using Xunit;

namespace Synonms.RestEasy.Testing.Tests;

public abstract partial class AuthenticatedTestFixture<TWebApplicationFactory, TEntryPoint, TAggregateRoot, TResource>
{
    [Fact]
    public void GetById_KnownId_Returns200Ok() =>
        GetByIdTest<TAggregateRoot, TResource>.Create(WebApplicationFactory.CreateClient(), CollectionPath, JsonSerialiserOptions, PersistAggregateAsync, ValidateResource)
            .Arrange.WithAggregate(GenerateUniqueAggregate())
            .Act.WithAuthenticatedUser(UserId)
            .Assert.SucceedsWith(HttpStatusCode.OK);
    
    [Fact]
    public void GetById_UnknownId_Returns404NotFound() =>
        GetByIdTest<TAggregateRoot, TResource>.Create(WebApplicationFactory.CreateClient(), CollectionPath, JsonSerialiserOptions, PersistAggregateAsync, ValidateResource)
            .Arrange.WithoutAggregate()
            .Act
                .WithAuthenticatedUser(UserId)
                .WithId(EntityId<TAggregateRoot>.New())
            .Assert.FailsWith(HttpStatusCode.NotFound);
    
    [Fact]
    public void GetById_WithoutUser_Returns401Unauthorized() =>
        GetByIdTest<TAggregateRoot, TResource>.Create(WebApplicationFactory.CreateClient(), CollectionPath, JsonSerialiserOptions, PersistAggregateAsync, ValidateResource)
            .Arrange.WithAggregate(GenerateUniqueAggregate())
            .Act
            .Assert.FailsWith(HttpStatusCode.Unauthorized);
}