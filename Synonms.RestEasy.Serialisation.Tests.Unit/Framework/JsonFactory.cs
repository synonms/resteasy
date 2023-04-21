namespace Synonms.RestEasy.Serialisation.Tests.Unit.Framework;

public static class JsonFactory
{
    public static string CreateResource(Guid id) =>
        $@"{{
                ""id"": ""{id}"",
                ""someBool"": {ResourceFactory.SomeBool.ToString().ToLowerInvariant()},
                ""someInt"": {ResourceFactory.SomeInt},
                ""someString"": ""{ResourceFactory.SomeString}"",
                ""self"": {{
                    ""href"": ""http://localhost:5000/resources/{id}"",
                    ""method"": ""GET"",
                    ""rel"": ""self""
                }},
                ""edit-form"": {{
                    ""href"": ""http://localhost:5000/resources/{id}/edit-form"",
                    ""method"": ""GET"",
                    ""rel"": ""edit-form""
                }},
                ""delete"": {{
                    ""href"": ""http://localhost:5000/resources/{id}"",
                    ""method"": ""DELETE"",
                    ""rel"": ""self""
                }}
            }}";

    public static string CreateResourceDocument(Guid id) =>
        $@"{{
                ""value"": {CreateResource(id)},
                ""self"": {{
                    ""href"": ""http://localhost:5000/resources/{id}"",
                    ""method"": ""GET"",
                    ""rel"": ""self""
                }}
            }}";

    public static string CreateResourceCollectionDocument(Guid id1, Guid id2) =>
        $@"{{
                ""value"": [{CreateResource(id1)},{CreateResource(id2)}],
                ""self"": {{
                    ""href"": ""http://localhost:5000/resources"",
                    ""method"": ""GET"",
                    ""rel"": ""self""
                }},
                {CreatePagination()}
            }}";

    public static string CreatePagination() => 
        $@"
            ""offset"": {PaginationFactory.Offset},
            ""limit"": {PaginationFactory.Limit},
            ""size"": {PaginationFactory.Size},
            ""first"": {{
                ""href"": ""{PaginationFactory.FirstLink.Uri.OriginalString}"",
                ""rel"": ""collection"",
                ""method"": ""GET""
            }},
            ""last"": {{
                ""href"": ""{PaginationFactory.LastLink.Uri.OriginalString}"",
                ""rel"": ""collection"",
                ""method"": ""GET""
            }},
            ""next"": {{
                ""href"": ""{PaginationFactory.NextLink.Uri.OriginalString}"",
                ""rel"": ""collection"",
                ""method"": ""GET""
            }}
        ";
}