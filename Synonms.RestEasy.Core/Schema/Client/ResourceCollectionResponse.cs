using Synonms.Functional;
using Synonms.RestEasy.Core.Schema.Errors;
using Synonms.RestEasy.Core.Schema.Resources;

namespace Synonms.RestEasy.Core.Schema.Client;

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