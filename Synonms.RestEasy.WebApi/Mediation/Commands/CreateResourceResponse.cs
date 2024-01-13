using Synonms.RestEasy.Core.Domain;
using Synonms.Functional;

namespace Synonms.RestEasy.WebApi.Mediation.Commands;

public class CreateResourceResponse<TAggregateRoot> 
    where TAggregateRoot : AggregateRoot<TAggregateRoot>
{
    public CreateResourceResponse(Result<TAggregateRoot> outcome)
    {
        Outcome = outcome;
    }

    public Result<TAggregateRoot> Outcome { get; }
}