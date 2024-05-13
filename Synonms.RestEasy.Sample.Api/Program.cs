using Synonms.RestEasy.Sample.Api;
using Synonms.RestEasy.Sample.Api.Addresses;
using Synonms.RestEasy.Sample.Api.Contracts;
using Synonms.RestEasy.Sample.Api.Employees;
using Synonms.RestEasy.Sample.Api.Infrastructure;
using Synonms.RestEasy.WebApi.Startup;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.OpenApi.Models;
using Synonms.RestEasy.Core.Domain;
using Synonms.RestEasy.Core.Persistence;
using Synonms.RestEasy.WebApi.Hypermedia.Default;
using Synonms.RestEasy.WebApi.Hypermedia.Ion;
using Synonms.RestEasy.WebApi.Pipeline.Products.Persistence;
using Synonms.RestEasy.WebApi.Pipeline.Tenants.Persistence;
using Synonms.RestEasy.WebApi.Pipeline.Users.Persistence;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddLogging();

ILoggerFactory loggerFactory = new LoggerFactory();

RestEasyOptions options = new()
{
    Assemblies = [SampleApiProject.Assembly],
    MvcOptionsConfigurationAction = mvcOptions => mvcOptions.ClearFormatters().WithDefaultFormatters(loggerFactory).WithIonFormatters(loggerFactory),
    SwaggerGenConfigurationAction = swaggerGenOptions => swaggerGenOptions.SwaggerDoc("v1.0", new OpenApiInfo { Title = "RestEasy Sample API", Version = "v1.0" }),
    SwaggerUiConfigurationAction = swaggerUiOptions => swaggerUiOptions.SwaggerEndpoint("/swagger/v1.0/swagger.json", "v1.0")
};

builder.Services.AddRestEasyFramework<SampleUser, SampleProduct, SampleTenant>(options);

builder.Services.AddSingleton<ILookupOptionsProvider, LookupOptionsProvider>();

builder.Services.AddScoped<IProductRepository<SampleProduct>, SampleProductRepository>();
builder.Services.AddScoped<ITenantRepository<SampleTenant>, SampleTenantRepository>();
builder.Services.AddScoped<IUserRepository<SampleUser>, SampleUserRepository>();

// Hack to persist in-memory data between HTTP requests
builder.Services.Replace(new ServiceDescriptor(typeof(IAggregateRepository<Address>), typeof(AddressesRepository), ServiceLifetime.Singleton));
builder.Services.Replace(new ServiceDescriptor(typeof(IAggregateRepository<Contract>), typeof(ContractsRepository), ServiceLifetime.Singleton));
builder.Services.Replace(new ServiceDescriptor(typeof(IAggregateRepository<Employee>), typeof(EmployeesRepository), ServiceLifetime.Singleton));

WebApplication app = builder.Build();

app.UseRestEasyFramework<SampleUser, SampleProduct, SampleTenant>(options);

app.Run();

public partial class Program { }    // Required for integration testing with WebApplicationFactory