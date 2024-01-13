using Synonms.RestEasy.Sample.Api;
using Synonms.RestEasy.Sample.Api.Addresses;
using Synonms.RestEasy.Sample.Api.Contracts;
using Synonms.RestEasy.Sample.Api.Employees;
using Synonms.RestEasy.Sample.Api.Infrastructure;
using Synonms.RestEasy.WebApi.Domain;
using Synonms.RestEasy.WebApi.Hypermedia.Default;
using Synonms.RestEasy.WebApi.Hypermedia.Ion;
using Synonms.RestEasy.WebApi.Startup;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.OpenApi.Models;
using Synonms.RestEasy.Core.Persistence;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddLogging();

ILoggerFactory loggerFactory = new LoggerFactory();

builder.Services.AddRestEasyFramework(mvcOptions => mvcOptions.WithDefaultFormatters(loggerFactory).WithIonFormatters(loggerFactory), SampleApiProject.Assembly)
    .WithApplicationDependenciesFrom(SampleApiProject.Assembly)
    .WithDomainDependenciesFrom(SampleApiProject.Assembly)
    .WithOpenApi(swaggerGenOptions =>
    {
        swaggerGenOptions.SwaggerDoc("v1.0", new OpenApiInfo { Title = "RestEasy Sample API", Version = "v1.0" });
    });

builder.Services.AddSingleton<ILookupOptionsProvider, LookupOptionsProvider>();

// Hack to persist in-memory data between HTTP requests
builder.Services.Replace(new ServiceDescriptor(typeof(IAggregateRepository<Address>), typeof(AddressesRepository), ServiceLifetime.Singleton));
builder.Services.Replace(new ServiceDescriptor(typeof(IAggregateRepository<Contract>), typeof(ContractsRepository), ServiceLifetime.Singleton));
builder.Services.Replace(new ServiceDescriptor(typeof(IAggregateRepository<Employee>), typeof(EmployeesRepository), ServiceLifetime.Singleton));

WebApplication app = builder.Build();

app.UseRestEasyFramework()
    .WithOpenApi(options =>
    {
        options.SwaggerEndpoint("/swagger/v1.0/swagger.json", "v1.0");
    })
    .WithControllers();

app.Run();

public partial class Program { }    // Required for integration testing with WebApplicationFactory