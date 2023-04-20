using Synonms.Functional;
using Synonms.RestEasy.Abstractions.Schema.Documents;
using Synonms.RestEasy.Abstractions.Schema.Errors;

namespace Synonms.RestEasy.Abstractions.Schema.Client;

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