using Synonms.Functional;

namespace Synonms.RestEasy.Abstractions.Schema;

public interface IErrorCollectionDocumentFactory
{
    ErrorCollectionDocument Create(Fault fault, Link requestedDocumentLink);
}