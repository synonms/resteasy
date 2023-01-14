using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Synonms.RestEasy.Swashbuckle;

public class ResourceDocumentFilter : IDocumentFilter
{
    private const string ValueRouteParameter = "{Value}";
    private const string IdRouteParameter = "{id}";
    private const string ValueParameterName = "Value";
    private const string IdParameterName = "id";
    private static readonly string[] ParameterNamesToExclude = { "IsEmpty" };

    // private const string PostResourceSuffix = "ForPost";
    // private const string PutResourceSuffix = "ForPut";

    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        List<string> keysToRemove = swaggerDoc.Paths
            .Where(x => x.Key.Contains(ValueRouteParameter))
            .Select(x => x.Key)
            .ToList();
        
        Dictionary<string, OpenApiPathItem> newPathItems = swaggerDoc.Paths
            .Where(x => x.Key.Contains(ValueRouteParameter))
            .Select(x => new { Key = x.Key.Replace(ValueRouteParameter, IdRouteParameter), x.Value })
            .ToDictionary(x => x.Key, x => x.Value);

        foreach (string key in keysToRemove)
        {
            swaggerDoc.Paths.Remove(key);
        }

        foreach ((string key, OpenApiPathItem value) in newPathItems)
        {
            foreach (KeyValuePair<OperationType,OpenApiOperation> openApiOperation in value.Operations)
            {
                List<OpenApiParameter> parametersToRemove = openApiOperation.Value.Parameters
                    .Where(x => ParameterNamesToExclude.Contains(x.Name))
                    .ToList();

                foreach (OpenApiParameter openApiParameter in parametersToRemove)
                {
                    openApiOperation.Value.Parameters.Remove(openApiParameter);
                }
                
                foreach (OpenApiParameter openApiParameter in openApiOperation.Value.Parameters)
                {
                    if (openApiParameter.Name.Equals(ValueParameterName))
                    {
                        openApiParameter.Name = IdParameterName;
                    }
                }
            }

            swaggerDoc.Paths[key] = value;
        }
        
        // foreach ((string path, OpenApiPathItem pathItem) in swaggerDoc.Paths)
        // {
        //     if (pathItem.Operations.ContainsKey(OperationType.Post))
        //     {
        //         HandleOperations(swaggerDoc, pathItem, OperationType.Post, PostResourceSuffix, PruneResourceSchemaForPost);
        //     }
        //
        //     if (pathItem.Operations.ContainsKey(OperationType.Put))
        //     {
        //         HandleOperations(swaggerDoc, pathItem, OperationType.Put, PutResourceSuffix, PruneResourceSchemaForPut);
        //     }
        // }
    }

    private static void HandleOperations(
        OpenApiDocument swaggerDoc,
        OpenApiPathItem pathItem,
        OperationType operationType,
        string suffix,
        Func<OpenApiDocument, string, OpenApiSchema> pruneResourceSchemaMethod)
    {
        OpenApiOperation operation = pathItem.Operations[operationType];

        foreach (var (_, mediaType) in operation.RequestBody.Content)
        {
            string resourceKey = mediaType.Schema.Reference.Id;

            if (resourceKey.EndsWith(suffix))
            {
                continue;
            }

            if (!swaggerDoc.Components.Schemas.ContainsKey(resourceKey))
            {
                continue;
            }

            string resourceForOperationKey = $"{resourceKey}{suffix}";

            if (!swaggerDoc.Components.Schemas.ContainsKey(resourceForOperationKey))
            {
                OpenApiSchema schemeForOperation = pruneResourceSchemaMethod(swaggerDoc, resourceKey);
                swaggerDoc.Components.Schemas.Add(resourceForOperationKey, schemeForOperation);
            }

            mediaType.Schema.Reference.Id = resourceForOperationKey;
        }
    }

    private static OpenApiSchema PruneResourceSchemaForPost(OpenApiDocument swaggerDoc, string resourceKey)
    {
        OpenApiSchema schema = swaggerDoc.Components.Schemas[resourceKey];
//        schema.Properties.Remove(nameof(ResourceIdentifier.Id).ToCamelCase());
//        schema.Properties.Remove(nameof(ResourceIdentifier.Type).ToCamelCase());

        return schema;
    }
        
    private static OpenApiSchema PruneResourceSchemaForPut(OpenApiDocument swaggerDoc, string resourceKey)
    {
        OpenApiSchema schema = swaggerDoc.Components.Schemas[resourceKey];
//        schema.Properties.Remove(nameof(ResourceIdentifier.Id).ToCamelCase());
//        schema.Properties.Remove(nameof(ResourceIdentifier.Type).ToCamelCase());

        return schema;
    }
}