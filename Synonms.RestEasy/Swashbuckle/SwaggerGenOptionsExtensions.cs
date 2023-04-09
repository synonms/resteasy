using Swashbuckle.AspNetCore.SwaggerGen;

namespace Synonms.RestEasy.Swashbuckle;

public static class SwaggerGenOptionsExtensions
{
    public static void ConfigureForIon(this SwaggerGenOptions options)
    {
//        options.SchemaFilter<IonSchemaFilter>();
    }
}