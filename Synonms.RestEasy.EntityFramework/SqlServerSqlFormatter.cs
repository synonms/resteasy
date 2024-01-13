using Synonms.RestEasy.Core.Persistence;

namespace Synonms.RestEasy.EntityFramework;

public class SqlServerSqlFormatter : ISqlFormatter
{
    public string FormatTableName(string schemaName, string tableName, string? alias = null) =>
        schemaName + "." + tableName + (string.IsNullOrWhiteSpace(alias) ? string.Empty : " AS " + alias);
    
    public string FormatLimitStatement(int numberOfRows) => string.Empty;
    
    public string FormatTopStatement(int numberOfRows) => "TOP " + numberOfRows;
    
    public string FormatWithStatements(IDictionary<string, string> withStatements) =>
        withStatements.Any()
            ? "WITH " + string.Join(", ", withStatements.Select(x => x.Key + " AS (" + x.Value + ")"))
            : string.Empty;
}