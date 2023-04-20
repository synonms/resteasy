using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Synonms.RestEasy.Abstractions.Attributes;
using Synonms.RestEasy.Abstractions.Constants;
using Synonms.RestEasy.Abstractions.Routing;

namespace Synonms.RestEasy.IoC;

public class RestEasyControllerModelConvention : IControllerModelConvention
{
    private readonly IRouteNameProvider _routeNameProvider;

    public RestEasyControllerModelConvention(IRouteNameProvider routeNameProvider)
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
            AddActionRoutes(controllerModel, aggregateRootType);
        }
    }

    private void AddActionRoutes(ControllerModel controllerModel, Type aggregateRootType)
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
        }
    }

    private static void AddControllerRoute(ControllerModel controllerModel, RestEasyResourceAttribute resourceAttribute)
    {
        if (string.IsNullOrWhiteSpace(resourceAttribute.CollectionPath) is false)
        {
            controllerModel.Selectors.Add(new SelectorModel
            {
                AttributeRouteModel = new AttributeRouteModel(new RouteAttribute(resourceAttribute.CollectionPath)),
            });
        }
    }
}
