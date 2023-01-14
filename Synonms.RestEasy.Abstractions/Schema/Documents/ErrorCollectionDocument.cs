namespace Synonms.RestEasy.Abstractions.Schema.Documents
{
    public class ErrorCollectionDocument : Document
    {
        public ErrorCollectionDocument(Link selfUri, IEnumerable<Error> errors)
            : base(selfUri)
        {
            Errors = errors;
        }

        public IEnumerable<Error> Errors { get; }
    }
}