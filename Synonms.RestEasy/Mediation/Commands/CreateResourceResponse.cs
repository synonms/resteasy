using Synonms.Functional;
using Synonms.RestEasy.Abstractions.Domain;

namespace Synonms.RestEasy.Mediation.Commands;

public class CreateResourceResponse<TAggregateRoot> 
    where TAggregateRoot : AggregateRoot<TAggregateRoot>
{
    public CreateResourceResponse(Result<TAggregateRoot> outcome)
    {
        Outcome = outcome;
    }

    public Result<TAggregateRoot> Outcome { get; }
}