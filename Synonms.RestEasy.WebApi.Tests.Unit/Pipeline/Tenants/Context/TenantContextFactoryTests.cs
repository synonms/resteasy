using System;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using Synonms.Functional;
using Synonms.RestEasy.WebApi.Pipeline.Tenants.Context;
using Synonms.RestEasy.WebApi.Pipeline.Tenants.Persistence;
using Synonms.RestEasy.WebApi.Tests.Unit.Shared;
using Xunit;

namespace Synonms.RestEasy.WebApi.Tests.Unit.Pipeline.Tenants.Context;

public class TenantContextFactoryTests
{
    private readonly ITenantRepository<TestTenant> _mockTenantRepository = Substitute.For<ITenantRepository<TestTenant>>();

    private readonly TestTenant _tenant = new()
    {
        Id = Guid.NewGuid(),
        Name = "Tenant Education"
    };

    public TenantContextFactoryTests()
    {
        _mockTenantRepository
            .FindSelectedTenantAsync(_tenant.Id, Arg.Any<CancellationToken>())
            .Returns(Maybe<TestTenant>.Some(_tenant));

        _mockTenantRepository
            .FindAvailableTenantsAsync(Arg.Any<CancellationToken>())
            .Returns([_tenant]);
    }
        
    [Fact]
    public async Task CreateAsync_WithValidSelectedTenantId_ThenReturnsSuccessWithTenantInfo()
    {
        TenantContextFactory<TestTenant> factory = new(_mockTenantRepository);

        Result<TenantContext<TestTenant>> result = await factory.CreateAsync(_tenant.Id, CancellationToken.None);

        result.Match(
            tenantContext =>
            {
                Assert.NotNull(tenantContext.SelectedTenant);
                Assert.Equal(_tenant.Id, tenantContext.SelectedTenant.Id);
                Assert.Equal(_tenant.Name, tenantContext.SelectedTenant.Name);
                Assert.Collection(tenantContext.AvailableTenants,
                    tenant =>
                    {
                        Assert.Equal(_tenant.Id, tenant.Id);
                        Assert.Equal(_tenant.Name, tenant.Name);
                    });
            }, 
            fault => Assert.Fail($"Expected Success but got Fault: {fault}"));
    }

    [Fact]
    public async Task CreateAsync_WithInvalidSelectedTenantId_ThenReturnsSuccessWithoutTenantInfo()
    {
        TenantContextFactory<TestTenant> factory = new(_mockTenantRepository);

        _mockTenantRepository
            .FindSelectedTenantAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(Maybe<TestTenant>.None);

        Result<TenantContext<TestTenant>> result = await factory.CreateAsync(Guid.NewGuid(), CancellationToken.None);

        result.Match(
            tenantContext =>
            {
                Assert.Null(tenantContext.SelectedTenant);
                Assert.Collection(tenantContext.AvailableTenants,
                    tenant =>
                    {
                        Assert.Equal(_tenant.Id, tenant.Id);
                        Assert.Equal(_tenant.Name, tenant.Name);
                    });
            }, 
            fault => Assert.Fail($"Expected Success but got Fault: {fault}"));
    }
    
    [Fact]
    public async Task CreateAsync_WithoutSelectedTenantId_ThenReturnsSuccessWithoutTenantInfo()
    {
        TenantContextFactory<TestTenant> factory = new(_mockTenantRepository);

        Result<TenantContext<TestTenant>> result = await factory.CreateAsync(null, CancellationToken.None);

        result.Match(
            tenantContext =>
            {
                Assert.Null(tenantContext.SelectedTenant);
                Assert.Collection(tenantContext.AvailableTenants,
                    tenant =>
                    {
                        Assert.Equal(_tenant.Id, tenant.Id);
                        Assert.Equal(_tenant.Name, tenant.Name);
                    });
            }, 
            fault => Assert.Fail($"Expected Success but got Fault: {fault}"));
    }
}