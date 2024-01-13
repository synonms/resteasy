using System.Net;
using Synonms.RestEasy.WebApi.Schema.Forms;
using Xunit;

namespace Synonms.RestEasy.Testing.Tests;

public abstract partial class AnonymousTestFixture<TWebApplicationFactory, TEntryPoint, TAggregateRoot, TResource>
{
    [Fact]
    public virtual void CreateForm_Valid_Returns200Ok() =>
        CreateFormTest.Create(WebApplicationFactory.CreateClient(), CollectionPath, JsonSerialiserOptions, ValidateCreateForm)
            .Arrange
            .Act
            .Assert.SucceedsWith(HttpStatusCode.OK);
    
    protected abstract void ValidateCreateForm(Form form);
}