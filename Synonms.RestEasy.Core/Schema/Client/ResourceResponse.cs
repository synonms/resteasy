using Synonms.Functional;
using Synonms.RestEasy.Core.Schema.Errors;
using Synonms.RestEasy.Core.Schema.Resources;

namespace Synonms.RestEasy.Core.Schema.Client;

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