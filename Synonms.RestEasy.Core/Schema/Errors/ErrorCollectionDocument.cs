namespace Synonms.RestEasy.Core.Schema.Errors;

public class ErrorCollectionDocument : Document
{
    public ErrorCollectionDocument(Link selfUri, IEnumerable<Error> errors)
        : base(selfUri)
    {
        Errors = errors;
    }

    public IEnumerable<Error> Errors { get; }
}