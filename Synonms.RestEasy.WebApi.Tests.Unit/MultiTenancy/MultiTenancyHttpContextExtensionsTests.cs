using System;
using Synonms.RestEasy.WebApi.Http;
using Synonms.RestEasy.WebApi.MultiTenancy;
using Synonms.RestEasy.WebApi.Tests.Unit.Shared;
using Microsoft.AspNetCore.Http;
using Synonms.Functional;
using Xunit;

namespace Synonms.RestEasy.WebApi.Tests.Unit.MultiTenancy;

public class MultiTenancyHttpContextExtensionsTests
{
    private readonly Guid _expectedTenantId = Guid.NewGuid();
        
    [Fact]
    public void GetTenant_KeyMissing_ReturnsNone()
    {
        DefaultHttpContext httpContext = new();

        Maybe<TestTenant> maybe = httpContext.GetTenant<TestTenant>();

        Assert.True(maybe.IsNone);
    }

    [Fact]
    public void GetTenant_ValueInvalid_ReturnsNone()
    {
        DefaultHttpContext httpContext = new()
        {
            Items =
            {
                [HttpContextItemKeys.Tenant] = 123
            }
        };

        Maybe<TestTenant> maybe = httpContext.GetTenant<TestTenant>();

        Assert.True(maybe.IsNone);
    }

    [Fact]
    public void GetTenant_ValueNull_ReturnsNone()
    {
        DefaultHttpContext httpContext = new()
        {
            Items =
            {
                [HttpContextItemKeys.Tenant] = null
            }
        };
            
        Maybe<TestTenant> maybe = httpContext.GetTenant<TestTenant>();

        Assert.True(maybe.IsNone);
    }

    [Fact]
    public void GetTenant_ValueValid_ReturnsSome()
    {
        DefaultHttpContext httpContext = new()
        {
            Items =
            {
                [HttpContextItemKeys.Tenant] = new TestTenant
                {
                    Id = _expectedTenantId
                }
            }
        };

        Maybe<TestTenant> maybe = httpContext.GetTenant<TestTenant>();

        maybe.Match(
            actualTenant => Assert.Equal(_expectedTenantId, actualTenant.Id), 
            () => Assert.Fail("Expected Some"));
    }
}