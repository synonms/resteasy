using System.Collections.Immutable;

namespace Synonms.RestEasy.Core.Persistence.Migrations;

public interface IMigrationsProvider
{
    Task<IImmutableQueue<string>> GetUpsAsync(int fromVersion, int toVersion, CancellationToken cancellationToken);
    
    Task<IImmutableQueue<string>> GetDownsAsync(int fromVersion, int toVersion, CancellationToken cancellationToken);
}