using System.Net;
using Synonms.RestEasy.WebApi.Schema.Forms;
using Xunit;

namespace Synonms.RestEasy.Testing.Tests;

public abstract partial class AuthenticatedTestFixture<TWebApplicationFactory, TEntryPoint, TAggregateRoot, TResource>
{
    [Fact]
    public virtual void CreateForm_Valid_Returns200Ok() =>
        CreateFormTest.Create(WebApplicationFactory.CreateClient(), CollectionPath, JsonSerialiserOptions, ValidateCreateForm)
            .Arrange
            .Act.WithAuthenticatedUser(UserId)
            .Assert.SucceedsWith(HttpStatusCode.OK);

    [Fact]
    public virtual void CreateForm_WithoutUser_Returns401Unauthorized() =>
        CreateFormTest.Create(WebApplicationFactory.CreateClient(), CollectionPath, JsonSerialiserOptions, ValidateCreateForm)
            .Arrange
            .Act
            .Assert.FailsWith(HttpStatusCode.Unauthorized);
    
    protected abstract void ValidateCreateForm(Form form);
}