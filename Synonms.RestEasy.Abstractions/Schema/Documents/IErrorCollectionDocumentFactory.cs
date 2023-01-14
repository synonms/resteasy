using Synonms.Functional;

namespace Synonms.RestEasy.Abstractions.Schema.Documents;

public interface IErrorCollectionDocumentFactory
{
    ErrorCollectionDocument Create(Fault fault, Link requestedDocumentLink);
}