namespace Synonms.RestEasy.Core.Persistence.Migrations;

public interface IDatabaseMigrator
{
    Task MigrateAsync(int toVersion, IEnumerable<string> statements, CancellationToken cancellationToken);
}