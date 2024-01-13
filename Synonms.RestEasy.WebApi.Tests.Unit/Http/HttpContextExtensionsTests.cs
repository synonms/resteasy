using System;
using Synonms.RestEasy.WebApi.Http;
using Synonms.RestEasy.WebApi.Tests.Unit.Shared;
using Synonms.RestEasy.WebApi.Versioning;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace Synonms.RestEasy.WebApi.Tests.Unit.Http;

public class HttpContextExtensionsTests
{
    [Fact]
    public void GetApiVersion_ItemMissing_ReturnsDefault()
    {
        HttpContext httpContext = new DefaultHttpContext();
        
        int actualVersion = httpContext.GetApiVersion();

        Assert.Equal(VersioningConfiguration.DefaultVersion, actualVersion);
    }

    [Theory]
    [InlineData(123456.456789)]
    [InlineData("test")]
    [InlineData(12345678998765435142)]
    public void GetApiVersion_ItemNotAnInt_ReturnsDefault(object invalidVersion)
    {
        HttpContext httpContext = TestHttpContextFactory.CreateWithItem(HttpContextItemKeys.ApiVersion, invalidVersion);

        int actualVersion = httpContext.GetApiVersion();

        Assert.Equal(VersioningConfiguration.DefaultVersion, actualVersion);
    }
    
    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-1000)]
    public void GetApiVersion_ItemPresentWithInvalidVersion_ReturnsDefault(int invalidVersion)
    {
        HttpContext httpContext = TestHttpContextFactory.CreateWithItem(HttpContextItemKeys.ApiVersion, invalidVersion);

        int actualVersion = httpContext.GetApiVersion();

        Assert.Equal(VersioningConfiguration.DefaultVersion, actualVersion);
    }

    [Theory]
    [InlineData(2)]
    [InlineData(99)]
    [InlineData(1000)]
    public void GetApiVersion_ItemPresentWithValidVersion_ReturnsVersion(int expectedVersion)
    {
        HttpContext httpContext = TestHttpContextFactory.CreateWithItem(HttpContextItemKeys.ApiVersion, expectedVersion);

        int actualVersion = httpContext.GetApiVersion();

        Assert.Equal(expectedVersion, actualVersion);
    }

    [Fact]
    public void GetCorrelationId_ItemMissing_ReturnsNull()
    {
        HttpContext httpContext = new DefaultHttpContext();
        
        Guid? actualCorrelationId = httpContext.GetCorrelationId();

        Assert.Null(actualCorrelationId);
    }
    
    [Theory]
    [InlineData(123456.456789)]
    [InlineData("test")]
    [InlineData(12345678998765435142)]
    public void GetCorrelationId_ItemNotAGuid_ReturnsNull(object invalidId)
    {
        HttpContext httpContext = TestHttpContextFactory.CreateWithItem(HttpContextItemKeys.CorrelationId, invalidId);

        Guid? actualCorrelationId = httpContext.GetCorrelationId();

        Assert.Null(actualCorrelationId);
    }
    
    [Fact]
    public void GetCorrelationId_ItemPresentWithInvalidGuid_ReturnsNull()
    {
        Guid invalidGuid = Guid.Empty;
        
        HttpContext httpContext = TestHttpContextFactory.CreateWithItem(HttpContextItemKeys.CorrelationId, invalidGuid);

        Guid? actualCorrelationId = httpContext.GetCorrelationId();

        Assert.Null(actualCorrelationId);
    }

    [Fact]
    public void GetCorrelationId_ItemPresentWithValidGuid_ReturnsCorrelationId()
    {
        Guid expectedCorrelationId = Guid.NewGuid();
        
        HttpContext httpContext = TestHttpContextFactory.CreateWithItem(HttpContextItemKeys.CorrelationId, expectedCorrelationId);

        Guid? actualCorrelationId = httpContext.GetCorrelationId();

        Assert.Equal(expectedCorrelationId, actualCorrelationId);
    }
    
    [Fact]
    public void GetRequestId_ItemMissing_ReturnsNull()
    {
        HttpContext httpContext = new DefaultHttpContext();
        
        Guid? actualRequestId = httpContext.GetRequestId();

        Assert.Null(actualRequestId);
    }
    
    [Theory]
    [InlineData(123456.456789)]
    [InlineData("test")]
    [InlineData(12345678998765435142)]
    public void GetRequestId_ItemNotAGuid_ReturnsNull(object invalidId)
    {
        HttpContext httpContext = TestHttpContextFactory.CreateWithItem(HttpContextItemKeys.RequestId, invalidId);

        Guid? actualRequestId = httpContext.GetRequestId();

        Assert.Null(actualRequestId);
    }
    
    [Fact]
    public void GetRequestId_ItemPresentWithInvalidGuid_ReturnsNull()
    {
        Guid invalidGuid = Guid.Empty;
        
        HttpContext httpContext = TestHttpContextFactory.CreateWithItem(HttpContextItemKeys.RequestId, invalidGuid);

        Guid? actualRequestId = httpContext.GetRequestId();

        Assert.Null(actualRequestId);
    }

    [Fact]
    public void GetRequestId_ItemPresentWithValidGuid_ReturnsRequestId()
    {
        Guid expectedRequestId = Guid.NewGuid();
        
        HttpContext httpContext = TestHttpContextFactory.CreateWithItem(HttpContextItemKeys.RequestId, expectedRequestId);

        Guid? actualRequestId = httpContext.GetRequestId();

        Assert.Equal(expectedRequestId, actualRequestId);
    }
}