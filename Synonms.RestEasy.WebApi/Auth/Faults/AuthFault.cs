using Synonms.Functional;

namespace Synonms.RestEasy.WebApi.Auth.Faults;

public abstract class AuthFault : Fault
{
    protected AuthFault(string code, string detail, FaultSource source, params object?[] arguments) 
        : base(code, "Auth", detail, source, arguments)
    {
    }
}