using Synonms.Functional;
using Synonms.RestEasy.WebApi.Schema.Errors;
using Synonms.RestEasy.WebApi.Schema.Resources;

namespace Synonms.RestEasy.WebApi.Schema.Client;

public class ResourceResponse<TResource> : OneOf<ErrorCollectionDocument, ResourceDocument<TResource>>
    where TResource : Resource
{
    public ResourceResponse(ErrorCollectionDocument leftValue) : base(leftValue)
    {
    }

    public ResourceResponse(ResourceDocument<TResource> rightValue) : base(rightValue)
    {
    }
}