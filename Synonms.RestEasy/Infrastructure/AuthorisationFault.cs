using Synonms.Functional;
using Synonms.RestEasy.Abstractions.Infrastructure;

namespace Synonms.RestEasy.Infrastructure;

public class AuthorisationFault : InfrastructureFault
{
    public AuthorisationFault(string detail, params object?[] arguments)
        : this(detail, new FaultSource(), arguments)
    {
    }
        
    public AuthorisationFault(string detail, FaultSource source, params object?[] arguments)
        : base(nameof(AuthorisationFault), "Unauthorised", detail, source, arguments)
    {
    }
}