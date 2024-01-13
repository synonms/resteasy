using Synonms.Functional;
using Synonms.RestEasy.WebApi.Schema.Errors;
using Synonms.RestEasy.WebApi.Schema.Resources;

namespace Synonms.RestEasy.WebApi.Schema.Client;

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