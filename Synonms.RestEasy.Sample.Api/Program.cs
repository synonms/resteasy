using Synonms.RestEasy.Abstractions.Persistence;
using Synonms.RestEasy.IoC;
using Synonms.RestEasy.Sample.Api;
using Synonms.RestEasy.Sample.Api.Addresses;
using Synonms.RestEasy.Sample.Api.People;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddRestEasy(SampleApiProject.Assembly);

// TODO: Automate this
builder.Services.AddScoped<IReadRepository<Address>, AddressRepository>();
builder.Services.AddScoped<IReadRepository<Person>, PersonRepository>();

WebApplication app = builder.Build();

app.MapControllers();

app.Run();