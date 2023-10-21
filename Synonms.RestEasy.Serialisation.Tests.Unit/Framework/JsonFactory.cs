namespace Synonms.RestEasy.Serialisation.Tests.Unit.Framework;

public static class JsonFactory
{
    public static string CreateResource(Guid id, Guid childId, Guid otherId) =>
        $@"{{
                ""id"": ""{id}"",
                ""someBool"": {ResourceFactory.SomeBool.ToString().ToLowerInvariant()},
                ""someInt"": {ResourceFactory.SomeInt},
                ""someString"": ""{ResourceFactory.SomeString}"",
                ""someChild"": {CreateChildResource(childId)},
                ""someOtherId"": ""{otherId}"",
                ""someOther"": {CreateOtherResource(otherId)},
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

    public static string CreateOtherResource(Guid id) =>
        $@"{{
                ""id"": ""{id}"",
                ""someBool"": {ResourceFactory.SomeBool.ToString().ToLowerInvariant()},
                ""someInt"": {ResourceFactory.SomeInt},
                ""someString"": ""{ResourceFactory.SomeString}"",
                ""self"": {{
                    ""href"": ""http://localhost:5000/other-resources/{id}"",
                    ""method"": ""GET"",
                    ""rel"": ""self""
                }},
                ""edit-form"": {{
                    ""href"": ""http://localhost:5000/other-resources/{id}/edit-form"",
                    ""method"": ""GET"",
                    ""rel"": ""edit-form""
                }},
                ""delete"": {{
                    ""href"": ""http://localhost:5000/other-resources/{id}"",
                    ""method"": ""DELETE"",
                    ""rel"": ""self""
                }}
            }}";

    public static string CreateChildResource(Guid id) =>
        $@"{{
                ""id"": ""{id}"",
                ""someBool"": {ResourceFactory.SomeBool.ToString().ToLowerInvariant()},
                ""someInt"": {ResourceFactory.SomeInt},
                ""someString"": ""{ResourceFactory.SomeString}""
            }}";

    public static string CreateResourceDocument(Guid id, Guid childId, Guid otherId) =>
        $@"{{
                ""value"": {CreateResource(id, childId, otherId)},
                ""self"": {{
                    ""href"": ""http://localhost:5000/resources/{id}"",
                    ""method"": ""GET"",
                    ""rel"": ""self""
                }}
            }}";

    public static string CreateResourceCollectionDocument(Guid id1, Guid childId1, Guid otherId1, Guid id2, Guid childId2, Guid otherId2) =>
        $@"{{
                ""value"": [{CreateResource(id1, childId1, otherId1)},{CreateResource(id2, childId2, otherId2)}],
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
                ""href"": ""{PaginationFactory.NextLink?.Uri.OriginalString ?? string.Empty}"",
                ""rel"": ""collection"",
                ""method"": ""GET""
            }}
        ";
}