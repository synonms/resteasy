using Synonms.Functional;

namespace Synonms.RestEasy.Core.Schema.Errors;

public interface IErrorCollectionDocumentFactory
{
    ErrorCollectionDocument Create(Fault fault, Link requestedDocumentLink);
}