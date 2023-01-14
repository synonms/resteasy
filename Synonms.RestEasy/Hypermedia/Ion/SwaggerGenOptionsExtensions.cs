using Swashbuckle.AspNetCore.SwaggerGen;

namespace Synonms.RestEasy.Hypermedia.Ion;

public static class SwaggerGenOptionsExtensions
{
    public static void ConfigureForIon(this SwaggerGenOptions options)
    {
//        options.SchemaFilter<IonSchemaFilter>();
    }
}