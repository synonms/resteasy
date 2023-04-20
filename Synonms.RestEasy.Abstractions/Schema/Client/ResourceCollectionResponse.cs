using Synonms.Functional;
using Synonms.RestEasy.Abstractions.Schema.Documents;
using Synonms.RestEasy.Abstractions.Schema.Errors;

namespace Synonms.RestEasy.Abstractions.Schema.Client;

public class ResourceCollectionResponse<TResource> : OneOf<ErrorCollectionDocument, ResourceCollectionDocument<TResource>>
    where TResource : Resource
{
    public ResourceCollectionResponse(ErrorCollectionDocument leftValue) : base(leftValue)
    {
    }

    public ResourceCollectionResponse(ResourceCollectionDocument<TResource> rightValue) : base(rightValue)
    {
    }
}