using System.Net;
using Synonms.RestEasy.Core.Domain;
using Xunit;

namespace Synonms.RestEasy.Testing.Tests;

public abstract partial class AnonymousTestFixture<TWebApplicationFactory, TEntryPoint, TAggregateRoot, TResource>
{
    [Fact]
    public virtual void Delete_KnownId_Returns204NoContent() =>
        DeleteTest<TAggregateRoot>.Create(WebApplicationFactory.CreateClient(), CollectionPath, PersistAggregateAsync, RetrieveAggregateAsync)
            .Arrange.WithAggregate(GenerateUniqueAggregate())
            .Act
            .Assert.SucceedsWith(HttpStatusCode.NoContent);
    
    [Fact]
    public virtual void Delete_UnknownId_Returns404NotFound() =>
        DeleteTest<TAggregateRoot>.Create(WebApplicationFactory.CreateClient(), CollectionPath, PersistAggregateAsync, RetrieveAggregateAsync)
            .Arrange.WithoutAggregate()
            .Act.WithId(EntityId<TAggregateRoot>.New())
            .Assert.FailsWith(HttpStatusCode.NotFound);
}