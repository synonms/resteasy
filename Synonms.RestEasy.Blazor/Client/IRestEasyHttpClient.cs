using Synonms.Functional;
using Synonms.RestEasy.Abstractions.Schema;
using Synonms.RestEasy.Abstractions.Schema.Documents;
using Synonms.RestEasy.Abstractions.Schema.Forms;

namespace Synonms.RestEasy.Blazor.Client;

public interface IRestEasyHttpClient
{
    Task<Result<FormDocument>> CreateFormAsync(string uri, CancellationToken cancellationToken);

    Task<Maybe<Fault>> DeleteAsync(string uri, CancellationToken cancellationToken);

    Task<Result<FormDocument>> EditFormAsync(string uri, CancellationToken cancellationToken);
    
    Task<Result<ResourceCollectionDocument<TResource>>> GetAllAsync<TResource>(string uri, CancellationToken cancellationToken) where TResource : Resource;
    
    Task<Result<ResourceDocument<TResource>>> GetByIdAsync<TResource>(string uri, CancellationToken cancellationToken) where TResource : Resource;
    
    Task<Maybe<Fault>> PostAsync<TResource>(string uri, TResource resource, CancellationToken cancellationToken) where TResource : Resource;
    
    Task<Maybe<Fault>> PutAsync<TResource>(string uri, TResource resource, CancellationToken cancellationToken) where TResource : Resource;
}