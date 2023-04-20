using Synonms.Functional;
using Synonms.RestEasy.Abstractions.Schema;
using Synonms.RestEasy.Abstractions.Schema.Documents;
using Synonms.RestEasy.Abstractions.Schema.Errors;
using Synonms.RestEasy.Application.Faults;
using Synonms.RestEasy.Domain.Faults;

namespace Synonms.RestEasy.Schema.Documents;

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