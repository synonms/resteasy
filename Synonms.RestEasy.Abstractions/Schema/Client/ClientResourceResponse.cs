using Synonms.Functional;

namespace Synonms.RestEasy.Abstractions.Schema.Client;

public class ClientResourceResponse<TResource> : OneOf<ErrorCollectionDocument, ClientResourceDocument<TResource>>
    where TResource : ClientResource
{
    public ClientResourceResponse(ErrorCollectionDocument leftValue) : base(leftValue)
    {
    }

    public ClientResourceResponse(ClientResourceDocument<TResource> rightValue) : base(rightValue)
    {
    }
}