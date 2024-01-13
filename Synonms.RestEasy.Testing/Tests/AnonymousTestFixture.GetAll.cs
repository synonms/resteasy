using System.Net;
using Xunit;

namespace Synonms.RestEasy.Testing.Tests;

public abstract partial class AnonymousTestFixture<TWebApplicationFactory, TEntryPoint, TAggregateRoot, TResource>
{
    [Fact]
    public void GetAll_EntitiesExistExceedingLimit_Returns200OkWithLimitedEntities() =>
        GetAllTest<TAggregateRoot, TResource>.Create(WebApplicationFactory.CreateClient(), CollectionPath, JsonSerialiserOptions, PageLimit, PersistAggregateAsync, ValidateResource)
            .Arrange.WithAggregates(Enumerable.Range(0, PageLimit + 1).Select(_ => GenerateUniqueAggregate()).ToArray())
            .Act
            .Assert.SucceedsWith(HttpStatusCode.OK);

    [Fact]
    public void GetAll_EntitiesExistWithinLimit_Returns200OkWithAllEntities() =>
        GetAllTest<TAggregateRoot, TResource>.Create(WebApplicationFactory.CreateClient(), CollectionPath, JsonSerialiserOptions, PageLimit, PersistAggregateAsync, ValidateResource)
            .Arrange.WithAggregates(Enumerable.Range(0, PageLimit - 1).Select(_ => GenerateUniqueAggregate()).ToArray())
            .Act
            .Assert.SucceedsWith(HttpStatusCode.OK);

    [Fact]
    public void GetAll_NoEntitiesExist_Returns200OkWithEmptyCollection() =>
        GetAllTest<TAggregateRoot, TResource>.Create(WebApplicationFactory.CreateClient(), CollectionPath, JsonSerialiserOptions, PageLimit, PersistAggregateAsync, ValidateResource)
            .Arrange.WithoutAggregates()
            .Act
            .Assert.SucceedsWith(HttpStatusCode.OK);
}