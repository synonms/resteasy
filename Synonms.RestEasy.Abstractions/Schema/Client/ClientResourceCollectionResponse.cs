using Synonms.Functional;

namespace Synonms.RestEasy.Abstractions.Schema.Client;

public class ClientResourceCollectionResponse<TResource> : OneOf<ErrorCollectionDocument, ClientResourceCollectionDocument<TResource>>
    where TResource : ClientResource
{
    public ClientResourceCollectionResponse(ErrorCollectionDocument leftValue) : base(leftValue)
    {
    }

    public ClientResourceCollectionResponse(ClientResourceCollectionDocument<TResource> rightValue) : base(rightValue)
    {
    }
}