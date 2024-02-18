using System.Reflection;
using System.Text;
using Synonms.RestEasy.Core.Extensions;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Synonms.RestEasy.Core.Attributes;
using Synonms.RestEasy.Core.Constants;
using Synonms.RestEasy.Core.Serialisation.Default;
using Synonms.RestEasy.Core.Serialisation.Ion;
using Synonms.RestEasy.WebApi.Routing;
using Synonms.RestEasy.WebApi.Swashbuckle.Default;
using Synonms.RestEasy.WebApi.Swashbuckle.Ion;

namespace Synonms.RestEasy.WebApi.Swashbuckle;

/// <summary>
/// Endpoints have to be manually added for OpenApi as route info is dynamic and not available on the endpoints.
/// This filter is the top level entry point to do that.
/// </summary>
public class RestEasyDocumentFilter : IDocumentFilter
{
    private readonly IResourceDirectory _resourceDirectory;

    public RestEasyDocumentFilter(IResourceDirectory resourceDirectory)
    {
        _resourceDirectory = resourceDirectory;
    }
    
    public void Apply(OpenApiDocument openApiDocument, DocumentFilterContext documentFilterContext)
    {
        IReadOnlyDictionary<string, IResourceDirectory.AggregateRootLayout> aggregateRootLayouts = _resourceDirectory.GetAllRoots();

        foreach ((string collectionName, IResourceDirectory.AggregateRootLayout aggregateRootLayout) in aggregateRootLayouts)
        {
            RestEasyResourceAttribute? resourceAttribute = aggregateRootLayout.AggregateRootType.GetCustomAttribute<RestEasyResourceAttribute>();

            if (resourceAttribute is null)
            {
                continue;
            }
            
            OpenApiOperation getAllOperation = GetAllOperation(collectionName, resourceAttribute);

            OpenApiPathItem resourceCollectionPathItem = new() 
            {
                Operations = new Dictionary<OperationType, OpenApiOperation>()
                {
                    [OperationType.Get] = getAllOperation
                }
            };
            
            if (resourceAttribute.IsCreateDisabled is false)
            {
                OpenApiOperation postOperation = PostOperation(collectionName, resourceAttribute);

                resourceCollectionPathItem.Operations.Add(OperationType.Post, postOperation);
                
                OpenApiOperation createFormOperation = CreateFormOperation(collectionName, resourceAttribute);
                
                OpenApiPathItem createFormPathItem = new() 
                {
                    Operations = new Dictionary<OperationType, OpenApiOperation>()
                    {
                        [OperationType.Get] = createFormOperation
                    }
                };
                
                openApiDocument.Paths.Add("/" + collectionName + "/create-form", createFormPathItem);
            }

            openApiDocument.Paths.Add("/" + collectionName, resourceCollectionPathItem);
            
            OpenApiOperation getByIdOperation = GetByIdOperation(collectionName, resourceAttribute);
            
            OpenApiPathItem resourcePathItem = new() 
            {
                Parameters = new List<OpenApiParameter>()
                {
                    new OpenApiParameter()
                    {
                        Name = "id", 
                        In = ParameterLocation.Path, 
                        Description = "Unique identifier of the resource."
                    } 
                },
                Operations = new Dictionary<OperationType, OpenApiOperation>()
                {
                    [OperationType.Get] = getByIdOperation
                }
            };

            if (resourceAttribute.IsUpdateDisabled is false)
            {
                OpenApiOperation putOperation = PutOperation(collectionName, resourceAttribute);

                resourcePathItem.Operations.Add(OperationType.Put, putOperation);
                
                OpenApiOperation editFormOperation = EditFormOperation(collectionName, resourceAttribute);
                
                OpenApiPathItem editFormPathItem = new() 
                {
                    Parameters = new List<OpenApiParameter>()
                    {
                        new OpenApiParameter()
                        {
                            Name = "id", 
                            In = ParameterLocation.Path, 
                            Description = "Unique identifier of the resource."
                        } 
                    },
                    Operations = new Dictionary<OperationType, OpenApiOperation>()
                    {
                        [OperationType.Get] = editFormOperation
                    }
                };
                
                openApiDocument.Paths.Add("/" + collectionName + "/{id}/edit-form", editFormPathItem);
            }

            if (resourceAttribute.IsDeleteDisabled is false)
            {
                OpenApiOperation deleteOperation = DeleteOperation(collectionName, resourceAttribute);

                resourcePathItem.Operations.Add(OperationType.Delete, deleteOperation);
            }

            openApiDocument.Paths.Add("/" + collectionName + "/{id}", resourcePathItem);
        }
    }

    private static string CollectionNameToTag(string collectionName)
    {
        string[] tokens = collectionName.Split('-', StringSplitOptions.RemoveEmptyEntries);

        StringBuilder stringBuilder = new();

        bool hasTokens = false;
        
        foreach (string token in tokens)
        {
            if (hasTokens)
            {
                stringBuilder.Append(' ');
            }
            stringBuilder.Append(token.ToPascalCase());
            hasTokens = true;
        }

        return stringBuilder.ToString();
    }

    private static OpenApiOperation CreateFormOperation(string collectionName, RestEasyResourceAttribute resourceAttribute)
    {
        OpenApiOperation createFormOperation = new()
        {
            Summary = "Get a form describing how to add a new resource to a collection.",
            Tags = new List<OpenApiTag>
            {
                new OpenApiTag { Name = CollectionNameToTag(collectionName) }
            }
        };

        Dictionary<string, OpenApiSchema> defaultCreateFormProperties = new()
        {
            {
                DefaultPropertyNames.Value, new OpenApiSchema
                {
                    Type = "array", 
                    Items = DefaultOpenApiSchemaFactory.CreateForFormField()
                }
            },
            { IanaLinkRelations.Self, IonOpenApiSchemaFactory.CreateForLink() }
        };

        Dictionary<string, OpenApiSchema> ionCreateFormProperties = new()
        {
            { IonPropertyNames.Links.Uri, new OpenApiSchema() { Type = "string", Format = "uri" } },
            { IonPropertyNames.Links.Relation, new OpenApiSchema() { Type = "string" } },
            { IonPropertyNames.Links.Method, new OpenApiSchema() { Type = "string" } },
            {
                IonPropertyNames.Value, new OpenApiSchema()
                {
                    Type = "array", 
                    Items = IonOpenApiSchemaFactory.CreateForFormField()
                }
            },
            { IanaLinkRelations.Self, IonOpenApiSchemaFactory.CreateForLink() }
        };

        createFormOperation.Responses.Add("200", new OpenApiResponse()
        {
            Description = "Success",
            Content = new Dictionary<string, OpenApiMediaType>
            {
                [MediaTypes.Json] = new OpenApiMediaType
                    {
                        Schema = new OpenApiSchema
                        {
                            Type = "object",
                            AdditionalPropertiesAllowed = true,
                            Properties = defaultCreateFormProperties,
                        }
                    },
                [MediaTypes.Ion] = new OpenApiMediaType
                    {
                        Schema = new OpenApiSchema
                        {
                            Type = "object",
                            AdditionalPropertiesAllowed = true,
                            Properties = ionCreateFormProperties,
                        }
                    }
            }
        });

        if (resourceAttribute.AllowAnonymous is false)
        {
            createFormOperation.Security.Add(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference {
                            Type = ReferenceType.SecurityScheme,
                            Id = "bearerAuth"
                        }
                    }, new List<string>()
                }
            });
        }

        return createFormOperation;
    }

    private static OpenApiOperation DeleteOperation(string collectionName, RestEasyResourceAttribute resourceAttribute)
    {
        OpenApiOperation deleteOperation = new()
        {
            Summary = "Deletes an existing resource.",
            Tags = new List<OpenApiTag>
            {
                new OpenApiTag { Name = CollectionNameToTag(collectionName) }
            }
        };

        deleteOperation.Responses.Add("204", new OpenApiResponse()
        {
            Description = "Successfully deleted"
        });

        if (resourceAttribute.AllowAnonymous is false)
        {
            deleteOperation.Security.Add(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference {
                            Type = ReferenceType.SecurityScheme,
                            Id = "bearerAuth"
                        }
                    }, new List<string>()
                }
            });
        }

        return deleteOperation;
    }
    
    private static OpenApiOperation EditFormOperation(string collectionName, RestEasyResourceAttribute resourceAttribute)
    {
        OpenApiOperation editFormOperation = new()
        {
            Summary = "Get a form describing how to update an existing resource.",
            Tags = new List<OpenApiTag>
            {
                new OpenApiTag { Name = CollectionNameToTag(collectionName) }
            }
        };

        Dictionary<string, OpenApiSchema> defaultCreateFormProperties = new()
        {
            {
                DefaultPropertyNames.Value, new OpenApiSchema
                {
                    Type = "array", 
                    Items = DefaultOpenApiSchemaFactory.CreateForFormField()
                }
            },
            { IanaLinkRelations.Self, IonOpenApiSchemaFactory.CreateForLink() }
        };

        Dictionary<string, OpenApiSchema> ionCreateFormProperties = new()
        {
            { IonPropertyNames.Links.Uri, new OpenApiSchema() { Type = "string", Format = "uri" } },
            { IonPropertyNames.Links.Relation, new OpenApiSchema() { Type = "string" } },
            { IonPropertyNames.Links.Method, new OpenApiSchema() { Type = "string" } },
            {
                IonPropertyNames.Value, new OpenApiSchema()
                {
                    Type = "array", 
                    Items = IonOpenApiSchemaFactory.CreateForFormField()
                }
            },
            { IanaLinkRelations.Self, IonOpenApiSchemaFactory.CreateForLink() }
        };

        editFormOperation.Responses.Add("200", new OpenApiResponse()
        {
            Description = "Success",
            Content = new Dictionary<string, OpenApiMediaType>
            {
                [MediaTypes.Json] = new OpenApiMediaType
                    {
                        Schema = new OpenApiSchema
                        {
                            Type = "object",
                            AdditionalPropertiesAllowed = true,
                            Properties = defaultCreateFormProperties,
                        }
                    },
                [MediaTypes.Ion] = new OpenApiMediaType
                    {
                        Schema = new OpenApiSchema
                        {
                            Type = "object",
                            AdditionalPropertiesAllowed = true,
                            Properties = ionCreateFormProperties,
                        }
                    }
            }
        });

        if (resourceAttribute.AllowAnonymous is false)
        {
            editFormOperation.Security.Add(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference {
                            Type = ReferenceType.SecurityScheme,
                            Id = "bearerAuth"
                        }
                    }, new List<string>()
                }
            });
        }

        return editFormOperation;
    }
        
    private static OpenApiOperation GetAllOperation(string collectionName, RestEasyResourceAttribute resourceAttribute)
    {
        OpenApiOperation getAllOperation = new()
        {
            Summary = "Get a paged collection of resources.",
            Tags = new List<OpenApiTag>
            {
                new OpenApiTag { Name = CollectionNameToTag(collectionName) }
            }
        };

        Dictionary<string, OpenApiSchema> defaultResourceCollectionDocumentProperties = new()
        {
            {
                DefaultPropertyNames.Value, new OpenApiSchema
                {
                    Type = "array", 
                    Items = DefaultOpenApiSchemaFactory.CreateForResource(resourceAttribute)
                }
            },
            { DefaultPropertyNames.Pagination.Offset, new OpenApiSchema() { Type = "integer" } },
            { DefaultPropertyNames.Pagination.Limit, new OpenApiSchema() { Type = "integer" } },
            { DefaultPropertyNames.Pagination.Size, new OpenApiSchema() { Type = "integer" } }
        };

        Dictionary<string, OpenApiSchema> ionResourceCollectionDocumentProperties = new()
        {
            {
                IonPropertyNames.Value, new OpenApiSchema()
                {
                    Type = "array", 
                    Items = IonOpenApiSchemaFactory.CreateForResource(resourceAttribute)
                }
            },
            { IanaLinkRelations.Self, IonOpenApiSchemaFactory.CreateForLink() },
            { "first", IonOpenApiSchemaFactory.CreateForLink() },
            { "previous", IonOpenApiSchemaFactory.CreateForLink() },
            { "next", IonOpenApiSchemaFactory.CreateForLink() },
            { "last", IonOpenApiSchemaFactory.CreateForLink() },
            { "offset", new OpenApiSchema() { Type = "integer" } },
            { "limit", new OpenApiSchema() { Type = "integer" } },
            { "size", new OpenApiSchema() { Type = "integer" } }
        };

        if (resourceAttribute.IsCreateDisabled is false)
        {
            ionResourceCollectionDocumentProperties.Add("create-form", IonOpenApiSchemaFactory.CreateForLink());
        }
        
        getAllOperation.Responses.Add("200", new OpenApiResponse()
        {
            Description = "Success",
            Content = new Dictionary<string, OpenApiMediaType>
            {
                [MediaTypes.Json] = new OpenApiMediaType
                    {
                        Schema = new OpenApiSchema
                        {
                            Type = "object",
                            AdditionalPropertiesAllowed = true,
                            Properties = defaultResourceCollectionDocumentProperties,
                        }
                    },
                [MediaTypes.Ion] = new OpenApiMediaType
                    {
                        Schema = new OpenApiSchema
                        {
                            Type = "object",
                            AdditionalPropertiesAllowed = true,
                            Properties = ionResourceCollectionDocumentProperties,
                        }
                    }
            }
        });

        if (resourceAttribute.AllowAnonymous is false)
        {
            getAllOperation.Security.Add(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference {
                            Type = ReferenceType.SecurityScheme,
                            Id = "bearerAuth"
                        }
                    }, new List<string>()
                }
            });
        }

        return getAllOperation;
    }

    private static OpenApiOperation GetByIdOperation(string collectionName, RestEasyResourceAttribute resourceAttribute)
    {
        OpenApiOperation getByIdOperation = new()
        {
            Summary = "Get an individual resource by Id.",
            Tags = new List<OpenApiTag>
            {
                new OpenApiTag { Name = CollectionNameToTag(collectionName) }
            }
        };

        Dictionary<string, OpenApiSchema> defaultResourceDocumentProperties = new()
        {
            { DefaultPropertyNames.Value, DefaultOpenApiSchemaFactory.CreateForResource(resourceAttribute) },
            { IanaLinkRelations.Self, DefaultOpenApiSchemaFactory.CreateForLink() }
        };

        Dictionary<string, OpenApiSchema> ionResourceDocumentProperties = new()
        {
            { IonPropertyNames.Value, IonOpenApiSchemaFactory.CreateForResource(resourceAttribute) },
            { IanaLinkRelations.Self, IonOpenApiSchemaFactory.CreateForLink() }
        };
        
        getByIdOperation.Responses.Add("200", new OpenApiResponse()
        {
            Description = "Success",
            Content = new Dictionary<string, OpenApiMediaType>
            {
                [MediaTypes.Json] = new OpenApiMediaType
                    {
                        Schema = new OpenApiSchema
                        {
                            Type = "object",
                            AdditionalPropertiesAllowed = true,
                            Properties = defaultResourceDocumentProperties,
                        }
                    },
                [MediaTypes.Ion] = new OpenApiMediaType
                    {
                        Schema = new OpenApiSchema
                        {
                            Type = "object",
                            AdditionalPropertiesAllowed = true,
                            Properties = ionResourceDocumentProperties,
                        }
                    }
            }
        });

        if (resourceAttribute.AllowAnonymous is false)
        {
            getByIdOperation.Security.Add(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference {
                            Type = ReferenceType.SecurityScheme,
                            Id = "bearerAuth"
                        }
                    }, new List<string>()
                }
            });
        }

        return getByIdOperation;
    }
        
    private static OpenApiOperation PostOperation(string collectionName, RestEasyResourceAttribute resourceAttribute)
    {
        OpenApiOperation postOperation = new()
        {
            Summary = "Add a new resource to a collection.",
            Tags = new List<OpenApiTag>
            {
                new OpenApiTag { Name = CollectionNameToTag(collectionName) }
            },
            RequestBody = new OpenApiRequestBody
            {
                Content = new Dictionary<string, OpenApiMediaType>
                {
                    [MediaTypes.Json] = new OpenApiMediaType
                    {
                        Schema = new OpenApiSchema
                        {
                            Type = "object",
                            AdditionalPropertiesAllowed = true,
                            Properties = OpenApiSchemaFactory.GenerateResourceProperties(resourceAttribute)
                        }
                    },
                    [MediaTypes.Ion] = new OpenApiMediaType
                    {
                        Schema = new OpenApiSchema
                        {
                            Type = "object",
                            AdditionalPropertiesAllowed = true,
                            Properties = OpenApiSchemaFactory.GenerateResourceProperties(resourceAttribute)
                        }
                    }
                }
            }
        };

        postOperation.Responses.Add("201", new OpenApiResponse()
        {
            Description = "Successfully created"
        });

        if (resourceAttribute.AllowAnonymous is false)
        {
            postOperation.Security.Add(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference {
                            Type = ReferenceType.SecurityScheme,
                            Id = "bearerAuth"
                        }
                    }, new List<string>()
                }
            });
        }

        return postOperation;
    }
    
    private static OpenApiOperation PutOperation(string collectionName, RestEasyResourceAttribute resourceAttribute)
    {
        OpenApiOperation putOperation = new()
        {
            Summary = "Updates an existing resource.",
            Tags = new List<OpenApiTag>
            {
                new OpenApiTag { Name = CollectionNameToTag(collectionName) }
            },
            RequestBody = new OpenApiRequestBody
            {
                Content = new Dictionary<string, OpenApiMediaType>
                {
                    [MediaTypes.Json] = new OpenApiMediaType
                    {
                        Schema = new OpenApiSchema
                        {
                            Type = "object",
                            AdditionalPropertiesAllowed = true,
                            Properties = OpenApiSchemaFactory.GenerateResourceProperties(resourceAttribute)
                        }
                    },
                    [MediaTypes.Ion] = new OpenApiMediaType
                    {
                        Schema = new OpenApiSchema
                        {
                            Type = "object",
                            AdditionalPropertiesAllowed = true,
                            Properties = OpenApiSchemaFactory.GenerateResourceProperties(resourceAttribute)
                        }
                    }
                }
            }
        };

        putOperation.Responses.Add("204", new OpenApiResponse()
        {
            Description = "Successfully updated"
        });

        if (resourceAttribute.AllowAnonymous is false)
        {
            putOperation.Security.Add(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference {
                            Type = ReferenceType.SecurityScheme,
                            Id = "bearerAuth"
                        }
                    }, new List<string>()
                }
            });
        }

        return putOperation;
    }
}