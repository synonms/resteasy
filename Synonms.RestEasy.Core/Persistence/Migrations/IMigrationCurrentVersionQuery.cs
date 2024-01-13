using Synonms.RestEasy.Core.Domain;

namespace Synonms.RestEasy.Core.Persistence.Migrations;

public interface IMigrationCurrentVersionQuery : IDomainQuery
{
    Task<int> ExecuteAsync();
}