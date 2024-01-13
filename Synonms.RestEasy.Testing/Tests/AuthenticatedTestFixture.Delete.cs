using System.Net;
using Synonms.RestEasy.Core.Domain;
using Xunit;

namespace Synonms.RestEasy.Testing.Tests;

public abstract partial class AuthenticatedTestFixture<TWebApplicationFactory, TEntryPoint, TAggregateRoot, TResource>
{
    [Fact]
    public virtual void Delete_KnownId_Returns204NoContent() =>
        DeleteTest<TAggregateRoot>.Create(WebApplicationFactory.CreateClient(), CollectionPath, PersistAggregateAsync, RetrieveAggregateAsync)
            .Arrange.WithAggregate(GenerateUniqueAggregate())
            .Act.WithAuthenticatedUser(UserId)
            .Assert.SucceedsWith(HttpStatusCode.NoContent);
    
    [Fact]
    public virtual void Delete_UnknownId_Returns404NotFound() =>
        DeleteTest<TAggregateRoot>.Create(WebApplicationFactory.CreateClient(), CollectionPath, PersistAggregateAsync, RetrieveAggregateAsync)
            .Arrange.WithoutAggregate()
            .Act
            .WithAuthenticatedUser(UserId)
            .WithId(EntityId<TAggregateRoot>.New())
            .Assert.FailsWith(HttpStatusCode.NotFound);

    [Fact]
    public virtual void Delete_WithoutUser_Returns401Unauthorized() =>
        DeleteTest<TAggregateRoot>.Create(WebApplicationFactory.CreateClient(), CollectionPath, PersistAggregateAsync, RetrieveAggregateAsync)
            .Arrange.WithAggregate(GenerateUniqueAggregate())
            .Act
            .Assert.FailsWith(HttpStatusCode.Unauthorized);
}