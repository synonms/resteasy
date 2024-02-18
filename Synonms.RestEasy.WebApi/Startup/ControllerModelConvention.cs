using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Authorization;
using Synonms.RestEasy.Core.Attributes;
using Synonms.RestEasy.Core.Constants;
using Synonms.RestEasy.WebApi.Routing;

namespace Synonms.RestEasy.WebApi.Startup;

public class ControllerModelConvention : IControllerModelConvention
{
    private readonly IRouteNameProvider _routeNameProvider;

    public ControllerModelConvention(IRouteNameProvider routeNameProvider)
    {
        _routeNameProvider = routeNameProvider;
    }
    
    public void Apply(ControllerModel controllerModel)
    {
        if (controllerModel.ControllerType.IsGenericType)
        {
            Type aggregateRootType = controllerModel.ControllerType.GenericTypeArguments[0];
            RestEasyResourceAttribute? resourceAttribute = aggregateRootType.GetCustomAttribute<RestEasyResourceAttribute>();

            if (resourceAttribute is null)
            {
                return;
            }
            
            AddControllerRoute(controllerModel, resourceAttribute);
            AddActionRoutes(controllerModel, aggregateRootType, resourceAttribute);
        }
    }

    private void AddActionRoutes(ControllerModel controllerModel, Type aggregateRootType, RestEasyResourceAttribute resourceAttribute)
    {
        foreach (ActionModel action in controllerModel.Actions)
        {
            RouteAttribute routeAttribute = action.ActionName switch
            {
                "GetById" => new RouteAttribute("{id}")
                {
                    Name = _routeNameProvider.GetById(aggregateRootType)
                },
                "GetAll" => new RouteAttribute("")
                {
                    Name = _routeNameProvider.GetAll(aggregateRootType)
                },
                "Post" => new RouteAttribute("")
                {
                    Name = _routeNameProvider.Post(aggregateRootType)
                },
                "Put" => new RouteAttribute("")
                {
                    Name = _routeNameProvider.Put(aggregateRootType)
                },
                "Delete" => new RouteAttribute("")
                {
                    Name = _routeNameProvider.Delete(aggregateRootType)
                },
                "CreateForm" => new RouteAttribute(IanaLinkRelations.Forms.Create)
                {
                    Name = _routeNameProvider.CreateForm(aggregateRootType)
                },
                "EditForm" => new RouteAttribute("{id}/" + IanaLinkRelations.Forms.Edit)
                {
                    Name = _routeNameProvider.EditForm(aggregateRootType)
                },
                _ => throw new InvalidOperationException($"Unexpected controller action '{action.ActionName}'.")
            };
            
            action.Selectors.Add(new SelectorModel
            {
                AttributeRouteModel = new AttributeRouteModel(routeAttribute)
            });

            if (resourceAttribute.AllowAnonymous)
            {
                AllowAnonymousFilter anonymousFilter = new();
                action.Filters.Add(anonymousFilter);
            }
            else
            {
                string policySuffix = aggregateRootType.Name;
                AuthorizeFilter authorizeFilter = action.ActionName switch
                {
                    "GetById" => new AuthorizeFilter("Read" + policySuffix),
                    "GetAll" => new AuthorizeFilter("Read" + policySuffix),
                    "Post" => new AuthorizeFilter("Create" + policySuffix),
                    "Put" => new AuthorizeFilter("Update" + policySuffix),
                    "Delete" => new AuthorizeFilter("Delete" + policySuffix),
                    "CreateForm" => new AuthorizeFilter("Create" + policySuffix),
                    "EditForm" => new AuthorizeFilter("Update" + policySuffix),
                    _ => throw new InvalidOperationException($"Unexpected controller action '{action.ActionName}'.")
                };
                
                action.Filters.Add(authorizeFilter);
            }
        }
    }

    private static void AddControllerRoute(ControllerModel controllerModel, RestEasyResourceAttribute resourceAttribute)
    {
        if (string.IsNullOrWhiteSpace(resourceAttribute.CollectionPath) is false)
        {
            controllerModel.Selectors.Add(new SelectorModel
            {
                // TODO: Make base path configurable
                AttributeRouteModel = new AttributeRouteModel(new RouteAttribute(resourceAttribute.CollectionPath)),
            });
        }

        if (resourceAttribute.AllowAnonymous is false)
        {
            controllerModel.Filters.Add(new AuthorizeFilter());
        }
    }
}