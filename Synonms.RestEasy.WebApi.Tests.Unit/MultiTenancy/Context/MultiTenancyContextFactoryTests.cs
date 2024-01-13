using System;
using System.Threading;
using System.Threading.Tasks;
using Synonms.RestEasy.WebApi.MultiTenancy.Context;
using Synonms.RestEasy.WebApi.MultiTenancy.Faults;
using Synonms.RestEasy.WebApi.MultiTenancy.Persistence;
using Synonms.RestEasy.WebApi.Tests.Unit.Shared;
using NSubstitute;
using Synonms.Functional;
using Xunit;

namespace Synonms.RestEasy.WebApi.Tests.Unit.MultiTenancy.Context;

public class MultiTenancyContextFactoryTests
{
    private readonly ITenantRepository<TestTenant> _mockTenantRepository = Substitute.For<ITenantRepository<TestTenant>>();

    private readonly TestTenant _tenant = new()
    {
        Id = Guid.NewGuid(),
        Name = "Tenant Education"
    };

    public MultiTenancyContextFactoryTests()
    {
        _mockTenantRepository
            .FindAsync(_tenant.Id, Arg.Any<CancellationToken>())
            .Returns(Maybe<TestTenant>.Some(_tenant));
    }
        
    [Fact]
    public async Task CreateAsync_ValidTenant_ThenReturnsSuccess()
    {
        MultiTenancyContextFactory<TestTenant> factory = new(_mockTenantRepository);

        Result<MultiTenancyContext<TestTenant>> result = await factory.CreateAsync(_tenant.Id, CancellationToken.None);

        result.Match(
            multiTenancyContext =>
            {
                Assert.Equal(_tenant.Id, multiTenancyContext.Tenant.Id);
                Assert.Equal(_tenant.Name, multiTenancyContext.Tenant.Name);
            }, 
            fault => Assert.Fail($"Expected Success but got Fault: {fault}"));
    }
        
    [Fact]
    public async Task CreateAsync_InvalidTenant_ThenReturnsTenantResolutionFault()
    {
        Guid invalidTenantCode = Guid.NewGuid();

        _mockTenantRepository
            .FindAsync(invalidTenantCode, Arg.Any<CancellationToken>())
            .Returns(Maybe<TestTenant>.None);

        MultiTenancyContextFactory<TestTenant> factory = new(_mockTenantRepository);

        Result<MultiTenancyContext<TestTenant>> result = await factory.CreateAsync(invalidTenantCode, CancellationToken.None);

        result.Match(
            _ => Assert.Fail("Expected Failure"), 
            fault => Assert.IsType<TenantResolutionFault>(fault));
    }
}