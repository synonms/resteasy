using System.Net;
using Synonms.RestEasy.Core.Domain;
using Synonms.RestEasy.WebApi.Schema.Forms;
using Xunit;

namespace Synonms.RestEasy.Testing.Tests;

public abstract partial class AnonymousTestFixture<TWebApplicationFactory, TEntryPoint, TAggregateRoot, TResource>
{
    [Fact]
    public void EditForm_KnownId_Returns200Ok() =>
        EditFormTest<TAggregateRoot>.Create(WebApplicationFactory.CreateClient(), CollectionPath, JsonSerialiserOptions, PersistAggregateAsync, ValidateEditForm)
            .Arrange.WithAggregate(GenerateUniqueAggregate())
            .Act
            .Assert.SucceedsWith(HttpStatusCode.OK);
    
    [Fact]
    public void EditForm_UnknownId_Returns404NotFound() =>
        EditFormTest<TAggregateRoot>.Create(WebApplicationFactory.CreateClient(), CollectionPath, JsonSerialiserOptions, PersistAggregateAsync, ValidateEditForm)
            .Arrange.WithoutAggregate()
            .Act.WithId(EntityId<TAggregateRoot>.New())
            .Assert.FailsWith(HttpStatusCode.NotFound);
    
    protected abstract void ValidateEditForm(Form form, TAggregateRoot aggregateRoot);
}