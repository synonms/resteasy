using Synonms.Functional;

namespace Synonms.RestEasy.WebApi.Mediation.Commands;

public class DeleteResourceResponse
{
    public DeleteResourceResponse(Maybe<Fault> outcome)
    {
        Outcome = outcome;
    }

    public Maybe<Fault> Outcome { get; }
}