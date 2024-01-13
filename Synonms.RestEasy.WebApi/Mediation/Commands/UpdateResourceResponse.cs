using Synonms.RestEasy.Core.Domain;
using Synonms.Functional;

namespace Synonms.RestEasy.WebApi.Mediation.Commands;

public class UpdateResourceResponse<TAggregateRoot>
    where TAggregateRoot : AggregateRoot<TAggregateRoot>
{
    public UpdateResourceResponse(Result<TAggregateRoot> outcome)
    {
        Outcome = outcome;
    }

    public Result<TAggregateRoot> Outcome { get; }
}