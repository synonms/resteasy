using Synonms.RestEasy.Core.Domain;

namespace Synonms.RestEasy.Core.Persistence;

public interface ILookupRepository<TLookup> 
    where TLookup : Lookup
{
    Task<TLookup?> FindAsync(EntityId<Lookup> id);
    
    Task<IEnumerable<TLookup>> ReadAsync();
}