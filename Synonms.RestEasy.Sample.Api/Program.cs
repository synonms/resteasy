using Synonms.RestEasy.IoC;
using Synonms.RestEasy.Sample.Api;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddRestEasy(SampleApiProject.Assembly);

WebApplication app = builder.Build();

app.MapControllers();

app.Run();