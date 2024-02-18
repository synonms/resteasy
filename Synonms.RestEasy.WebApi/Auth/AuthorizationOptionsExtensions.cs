using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;

namespace Synonms.RestEasy.WebApi.Auth;

public static class AuthorizationOptionsExtensions
{
    public static AuthorizationOptions AddRestEasyAuthorisationPolicies(this AuthorizationOptions authorisationOptions, params Assembly[] policyAssemblies) =>
        AddRestEasyAuthorisationPolicies(authorisationOptions, policyAssemblies.AsEnumerable());

    public static AuthorizationOptions AddRestEasyAuthorisationPolicies(this AuthorizationOptions authorisationOptions, IEnumerable<Assembly> policyAssemblies)
    {
        foreach (Assembly policyAssembly in policyAssemblies)
        {
            List<Type> policyRegistrars = policyAssembly.GetTypes()
                .Where(x => !x.IsInterface && !x.IsAbstract && x.GetInterfaces().Contains(typeof(IPolicyRegistrar)))
                .ToList();

            foreach (Type policyRegistrarType in policyRegistrars)
            {
                IPolicyRegistrar? policyRegistrar = Activator.CreateInstance(policyRegistrarType) as IPolicyRegistrar;

                if (policyRegistrar is null)
                {
                    throw new InvalidOperationException($"Unable to construct policy registrar type [{policyRegistrarType}].");
                }
                
                policyRegistrar.Register(authorisationOptions);
            }
        }

        return authorisationOptions;
    }
}