using Synonms.Functional;

namespace Synonms.RestEasy.WebApi.Schema.Errors;

public interface IErrorCollectionDocumentFactory
{
    ErrorCollectionDocument Create(Fault fault, Link requestedDocumentLink);
}