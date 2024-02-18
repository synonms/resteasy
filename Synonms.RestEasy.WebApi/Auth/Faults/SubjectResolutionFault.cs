using Synonms.Functional;

namespace Synonms.RestEasy.WebApi.Auth.Faults;

public class SubjectResolutionFault : AuthFault
{
    public SubjectResolutionFault(string detail, params object?[] arguments) 
        : this(detail, new FaultSource(), arguments)
    {
    }

    public SubjectResolutionFault(string detail, FaultSource source, params object?[] arguments) 
        : base(nameof(SubjectResolutionFault), detail, source, arguments)
    {
    }
}