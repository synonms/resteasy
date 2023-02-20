using Synonms.Functional;
using Synonms.RestEasy.Abstractions.Infrastructure;

namespace Synonms.RestEasy.Infrastructure;

public class ConcurrencyFault : InfrastructureFault
{
    public ConcurrencyFault(string detail, params object?[] arguments)
        : this(detail, new FaultSource(), arguments)
    {
    }
        
    public ConcurrencyFault(string detail, FaultSource source, params object?[] arguments)
        : base(nameof(ConcurrencyFault), "Concurrency error", detail, source, arguments)
    {
    }
}