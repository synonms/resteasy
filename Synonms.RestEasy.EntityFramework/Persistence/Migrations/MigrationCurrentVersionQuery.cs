using Synonms.RestEasy.Core.Persistence.Migrations;
using Microsoft.EntityFrameworkCore;

namespace Synonms.RestEasy.EntityFramework.Persistence.Migrations;

public class MigrationCurrentVersionQuery : IMigrationCurrentVersionQuery
{
    private readonly RestEasyDbContext _restEasyDbContext;

    public MigrationCurrentVersionQuery(RestEasyDbContext restEasyDbContext)
    {
        _restEasyDbContext = restEasyDbContext;
    }
    
    public async Task<int> ExecuteAsync()
    {
        try
        {
            Migration? latestMigration = await _restEasyDbContext.Migrations.OrderByDescending(x => x.CreatedAt).FirstOrDefaultAsync();

            return latestMigration?.ToVersion ?? 0;
        }
        catch (Exception exception)
        {
            if (exception.Message.Equals("Invalid object name 'Framework.Migrations'."))
            {
                // If no migrations have been run yet the table won't exist
                return 0;
            }

            throw;
        }
    }
}