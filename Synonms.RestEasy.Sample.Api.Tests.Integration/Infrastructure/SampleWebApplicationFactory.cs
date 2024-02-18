using System.Net.Http.Headers;
using Synonms.RestEasy.Sample.Api.Addresses;
using Synonms.RestEasy.Sample.Api.Contracts;
using Synonms.RestEasy.Sample.Api.Employees;
using Synonms.RestEasy.Testing.Extensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Synonms.RestEasy.Core.Constants;
using Synonms.RestEasy.Core.Persistence;

namespace Synonms.RestEasy.Sample.Api.Tests.Integration.Infrastructure;

public class SampleWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureClient(HttpClient httpClient)
    {
        base.ConfigureClient(httpClient);

        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(MediaTypes.Ion));
        httpClient.DefaultRequestHeaders.Add(WebApi.Http.HttpHeaders.TenantId, "00000000-0000-0000-0000-000000000001");
    }
    
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(serviceCollection =>
        {
            serviceCollection.Replace(new ServiceDescriptor(typeof(IAggregateRepository<Address>), typeof(TestAddressesRepository), ServiceLifetime.Singleton));
            serviceCollection.Replace(new ServiceDescriptor(typeof(IAggregateRepository<Contract>), typeof(TestContractsRepository), ServiceLifetime.Singleton));
            serviceCollection.Replace(new ServiceDescriptor(typeof(IAggregateRepository<Employee>), typeof(TestEmployeesRepository), ServiceLifetime.Singleton));
        });
        
        builder.ConfigureTestServices(serviceCollection =>
        {
            serviceCollection.ReplaceAuthentication();
        });
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        
        DisposeAsync().ConfigureAwait(false).GetAwaiter().GetResult();
    }
}