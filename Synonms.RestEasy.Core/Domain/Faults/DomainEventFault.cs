using Synonms.Functional;

namespace Synonms.RestEasy.Core.Domain.Faults;

public class DomainEventFault : DomainFault
{
    public DomainEventFault(string detail, params object?[] arguments)
        : this(detail, new FaultSource(), arguments)
    {
    }

    public DomainEventFault(string detail, FaultSource source, params object?[] arguments)
        : base(nameof(DomainEventFault), "Domain event", detail, source, arguments)
    {
    }
}