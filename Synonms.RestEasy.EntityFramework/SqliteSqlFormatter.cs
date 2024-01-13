using Synonms.RestEasy.Core.Persistence;

namespace Synonms.RestEasy.EntityFramework;

public class SqliteSqlFormatter : ISqlFormatter
{
    public string FormatTableName(string schemaName, string tableName, string? alias = null) =>
        tableName + (string.IsNullOrWhiteSpace(alias) ? string.Empty : " AS " + alias);

    public string FormatLimitStatement(int numberOfRows) => "LIMIT " + numberOfRows;
    
    public string FormatTopStatement(int numberOfRows) => string.Empty;

    public string FormatWithStatements(IDictionary<string, string> withStatements) =>
        withStatements.Any()
            ? "WITH " + string.Join(", ", withStatements.Select(x => x.Key + " AS (" + x.Value + ")"))
            : string.Empty;
}