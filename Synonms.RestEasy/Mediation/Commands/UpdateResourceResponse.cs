using Synonms.Functional;
using Synonms.RestEasy.Abstractions.Domain;

namespace Synonms.RestEasy.Mediation.Commands;

public class UpdateResourceResponse<TAggregateRoot>
    where TAggregateRoot : AggregateRoot<TAggregateRoot>
{
    public UpdateResourceResponse(Result<TAggregateRoot> outcome)
    {
        Outcome = outcome;
    }

    public Result<TAggregateRoot> Outcome { get; }
}