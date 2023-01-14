using Synonms.Functional;

namespace Synonms.RestEasy.Mediation.Commands;

public class DeleteResourceResponse
{
    public DeleteResourceResponse(Maybe<Fault> outcome)
    {
        Outcome = outcome;
    }

    public Maybe<Fault> Outcome { get; }
}