using Synonms.Functional;

namespace Synonms.RestEasy.Abstractions.Schema.Errors;

public interface IErrorCollectionDocumentFactory
{
    ErrorCollectionDocument Create(Fault fault, Link requestedDocumentLink);
}