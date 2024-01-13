using Synonms.Functional;
using Synonms.RestEasy.Core.Domain;

namespace Synonms.RestEasy.Core.Persistence.Migrations;

public class Migration : AggregateRoot<Migration>
{
    private Migration(int toVersion)
    {
        ToVersion = toVersion;
    }

    public int ToVersion { get; private set; }

    internal static Result<Migration> Create(int toVersion) => 
        new Migration(toVersion);
}
