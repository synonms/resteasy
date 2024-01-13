using System.Data;
using System.Data.Common;
using Synonms.RestEasy.Core.Persistence.Migrations;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Synonms.RestEasy.EntityFramework.Persistence.Migrations;

public class DatabaseMigrator : IDatabaseMigrator
{
    private readonly RestEasyDbContext _restEasyDbContext;

    public DatabaseMigrator(RestEasyDbContext restEasyDbContext)
    {
        _restEasyDbContext = restEasyDbContext;
    }
    
    public async Task MigrateAsync(int toVersion, IEnumerable<string> statements, CancellationToken cancellationToken)
    {
        List<string> statementList = statements.ToList();

        DbConnection connection = _restEasyDbContext.Database.GetDbConnection();
        await connection.OpenAsync(cancellationToken);
        await using DbTransaction transaction  = await connection.BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken);

        try
        {
            foreach (string statement in statementList)
            {
                try
                {
                    DbCommand command = connection.CreateCommand();
                    command.CommandText = statement;
                    command.Transaction = transaction;
                    int _ = await command.ExecuteNonQueryAsync(cancellationToken);
                }
                catch (Exception exception)
                {
                    throw new Exception($"Exception occurred executing statement: [{statement}]", exception);
                }
            }

            if (toVersion > 0 && statementList.Any())
            {
                const string updateMigrationsSql = "insert into " + FrameworkDatabase.Schema + ".Migrations (ToVersion) values (@toVersion)";
                DbCommand updateMigrationsCommand = connection.CreateCommand();
                updateMigrationsCommand.CommandText = updateMigrationsSql;
                updateMigrationsCommand.Transaction = transaction;
                updateMigrationsCommand.Parameters.Add(new SqlParameter("@toVersion", toVersion));
                
                int updateMigrationsRowsAffected = await updateMigrationsCommand.ExecuteNonQueryAsync(cancellationToken);
            }

            await transaction.CommitAsync(cancellationToken);
        }
        catch (Exception)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
        finally 
        {
            await connection.CloseAsync();
        }
    }
}