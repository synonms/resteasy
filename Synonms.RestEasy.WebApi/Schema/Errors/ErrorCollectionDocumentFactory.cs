using Synonms.RestEasy.Core.Application.Faults;
using Synonms.RestEasy.Core.Domain.Faults;
using Synonms.Functional;

namespace Synonms.RestEasy.WebApi.Schema.Errors;

public class ErrorCollectionDocumentFactory : IErrorCollectionDocumentFactory
{
    public ErrorCollectionDocument Create(Fault fault, Link requestedDocumentLink)
    {
        IEnumerable<Error> errors = fault switch
        {
            AggregateFault aggregateFault => aggregateFault.Faults.Select(Map),
            ApplicationRulesFault applicationRulesFault => applicationRulesFault.Faults.Select(Map),
            DomainRulesFault domainRulesFault => domainRulesFault.Faults.Select(Map),
            _ => new List<Error>() { Map(fault) }
        };

        return new ErrorCollectionDocument(requestedDocumentLink, errors);
    }

    private static Error Map(Fault fault) =>
        new (fault.Id, fault.Code, fault.Title, string.Format(fault.Detail, fault.Arguments), new ErrorSource(fault.Source.Pointer, fault.Source.Parameter));
}