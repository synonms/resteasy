using Microsoft.OpenApi.Models;

namespace Synonms.RestEasy.Extensions;

public static class OpenApiPropertiesExtensions
{
    public static IDictionary<string, OpenApiSchema> RenameProperty(this IDictionary<string, OpenApiSchema> properties, string oldName, string newName)
    {
        if (oldName.Equals(newName, StringComparison.OrdinalIgnoreCase))
        {
            return properties;
        }
        
        properties[newName] = properties[oldName];
        properties.Remove(oldName);

        return properties;
    }
}