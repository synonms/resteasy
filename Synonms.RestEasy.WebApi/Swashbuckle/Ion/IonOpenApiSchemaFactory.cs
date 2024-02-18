using System.Reflection;
using Microsoft.OpenApi.Models;
using Synonms.RestEasy.Core.Attributes;
using Synonms.RestEasy.Core.Serialisation.Ion;

namespace Synonms.RestEasy.WebApi.Swashbuckle.Ion;

public static class IonOpenApiSchemaFactory
{
    public static OpenApiSchema CreateForFormField() =>
        new()
        {
            Type = "object",
            Properties = new Dictionary<string, OpenApiSchema>()
            {
                { IonPropertyNames.FormFields.Description, new OpenApiSchema() { Type = OpenApiDataTypes.String } },
                { IonPropertyNames.FormFields.Enabled, new OpenApiSchema() { Type = OpenApiDataTypes.Boolean } },
                { IonPropertyNames.FormFields.ElementType, new OpenApiSchema() { Type = OpenApiDataTypes.String } },
                { IonPropertyNames.FormFields.ElementForm, new OpenApiSchema() { Type = OpenApiDataTypes.Array } },
                { IonPropertyNames.FormFields.Form, new OpenApiSchema() { Type = OpenApiDataTypes.Array } },
                { IonPropertyNames.FormFields.Label, new OpenApiSchema() { Type = OpenApiDataTypes.String } },
                { IonPropertyNames.FormFields.Max, new OpenApiSchema() },
                { IonPropertyNames.FormFields.MaxLength, new OpenApiSchema() { Type = OpenApiDataTypes.Integer, Format = OpenApiIntegerFormats.Int32 } },
                { IonPropertyNames.FormFields.MaxSize, new OpenApiSchema() { Type = OpenApiDataTypes.Integer, Format = OpenApiIntegerFormats.Int32 } },
                { IonPropertyNames.FormFields.Min, new OpenApiSchema() },
                { IonPropertyNames.FormFields.MinLength, new OpenApiSchema() { Type = OpenApiDataTypes.Integer, Format = OpenApiIntegerFormats.Int32 } },
                { IonPropertyNames.FormFields.MinSize, new OpenApiSchema() { Type = OpenApiDataTypes.Integer, Format = OpenApiIntegerFormats.Int32 } },
                { IonPropertyNames.FormFields.Mutable, new OpenApiSchema() { Type = OpenApiDataTypes.Boolean } },
                { IonPropertyNames.FormFields.Name, new OpenApiSchema() { Type = OpenApiDataTypes.String } },
                { IonPropertyNames.FormFields.Options, new OpenApiSchema() { Type = OpenApiDataTypes.Array } },
                { IonPropertyNames.FormFields.Pattern, new OpenApiSchema() { Type = OpenApiDataTypes.String } },
                { IonPropertyNames.FormFields.Placeholder, new OpenApiSchema() { Type = OpenApiDataTypes.String } },
                { IonPropertyNames.FormFields.Required, new OpenApiSchema() { Type = OpenApiDataTypes.Boolean } },
                { IonPropertyNames.FormFields.Secret, new OpenApiSchema() { Type = OpenApiDataTypes.Boolean } },
                { IonPropertyNames.FormFields.Type, new OpenApiSchema() { Type = OpenApiDataTypes.String } },
                { IonPropertyNames.FormFields.Value, new OpenApiSchema() },
                { IonPropertyNames.FormFields.Visible, new OpenApiSchema() { Type = OpenApiDataTypes.Boolean } }
            }                          
        };
    
    public static OpenApiSchema CreateForResource(RestEasyResourceAttribute resourceAttribute)
    {
        Dictionary<string, OpenApiSchema> properties = OpenApiSchemaFactory.GenerateResourceProperties(resourceAttribute);

        foreach (PropertyInfo propertyInfo in resourceAttribute.ResourceType.GetProperties(BindingFlags.Instance | BindingFlags.Public))
        {
            if (propertyInfo.Name.Equals("id", StringComparison.OrdinalIgnoreCase)
                || propertyInfo.Name.Equals("CreatedAt", StringComparison.OrdinalIgnoreCase)
                || propertyInfo.Name.Equals("UpdatedAt", StringComparison.OrdinalIgnoreCase)
                || propertyInfo.Name.Equals("SelfLink", StringComparison.OrdinalIgnoreCase)
                || propertyInfo.Name.Equals("Links", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            // TODO: Add any other links
        }
        
        properties.Add("self", CreateForLink());

        if (resourceAttribute.IsUpdateDisabled is false)
        {
            properties.Add("edit-form", CreateForLink());
        }

        if (resourceAttribute.IsDeleteDisabled is false)
        {
            properties.Add("delete", CreateForLink());
        }

        OpenApiSchema schema = new()
        {
            Type = "object",
            Properties = properties
        };
        
        return schema;
    }
    
    public static OpenApiSchema CreateForLink() =>
        new()
        {
            Type = "object",
            Properties = new Dictionary<string, OpenApiSchema>()
            {
                { IonPropertyNames.Links.Uri, new OpenApiSchema() { Type = "string", Format = "uri" } },
                { IonPropertyNames.Links.Relation, new OpenApiSchema() { Type = "string" } },
                { IonPropertyNames.Links.Method, new OpenApiSchema() { Type = "string" } }
            }
        };
}