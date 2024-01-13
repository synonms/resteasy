namespace Synonms.RestEasy.Core.Persistence;

public interface ISqlFormatter
{
    string FormatTableName(string schemaName, string tableName, string? alias = null);
    
    string FormatLimitStatement(int numberOfRows);
    
    string FormatTopStatement(int numberOfRows);
    
    string FormatWithStatements(IDictionary<string, string> withStatements);
}