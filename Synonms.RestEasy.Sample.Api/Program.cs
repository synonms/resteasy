using Synonms.RestEasy.Abstractions.Domain;
using Synonms.RestEasy.Hypermedia.Ion;
using Synonms.RestEasy.IoC;
using Synonms.RestEasy.Sample.Api;
using Synonms.RestEasy.Sample.Api.Hacks;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddRestEasy(mvcOptions => mvcOptions.WithIon(), SampleApiProject.Assembly);
builder.Services.AddInMemoryRepositories();
builder.Services.AddSingleton<ILookupOptionsProvider, LookupOptionsProvider>();

WebApplication app = builder.Build();

app.MapControllers();

app.Run();