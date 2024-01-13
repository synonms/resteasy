using Synonms.RestEasy.Core.Persistence.Migrations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Synonms.RestEasy.WebApi.Controllers.Auth;

namespace Synonms.RestEasy.WebApi.Controllers;

[ApiController]
[Route(CollectionName)]
public class MigrationsController : ControllerBase
{
    public const string CollectionName = "migrations";
    
    private readonly IMigrationCurrentVersionQuery _migrationCurrentVersionQuery;
    private readonly IMigrationsProvider _migrationsProvider;
    private readonly IDatabaseMigrator _databaseMigrator;

    public MigrationsController(IMigrationCurrentVersionQuery migrationCurrentVersionQuery, IMigrationsProvider migrationsProvider, IDatabaseMigrator databaseMigrator)
    {
        _migrationCurrentVersionQuery = migrationCurrentVersionQuery;
        _migrationsProvider = migrationsProvider;
        _databaseMigrator = databaseMigrator;
    }

    [HttpGet]
    [Route("current-version")]
    [Authorize(Policy = MigrationsPolicyRegistrar.ReadPolicy)]
    public async Task<IActionResult> CurrentVersionAsync()
    {
        int currentVersion = await _migrationCurrentVersionQuery.ExecuteAsync();
        
        return Ok(new { version = currentVersion });
    }

    [HttpPost]
    [Route("upgrade/{toVersion:int}")]
    [Authorize(Policy = MigrationsPolicyRegistrar.CreatePolicy)]
    public async Task<IActionResult> ToVersionAsync([FromRoute] int toVersion, CancellationToken cancellationToken)
    {
        if (toVersion < 0)
        {
            return BadRequest();
        }

        int currentVersion = await _migrationCurrentVersionQuery.ExecuteAsync();

        if (toVersion == currentVersion)
        {
            return Ok(new { version = currentVersion });
        }

        List<string> statements = toVersion > currentVersion
            ? (await _migrationsProvider.GetUpsAsync(currentVersion, toVersion, cancellationToken)).ToList()
            : (await _migrationsProvider.GetDownsAsync(currentVersion, toVersion, cancellationToken)).ToList();

        await _databaseMigrator.MigrateAsync(toVersion, statements, cancellationToken);

        int updatedVersion = await _migrationCurrentVersionQuery.ExecuteAsync();

        return Ok(new { version = updatedVersion });
    }
}