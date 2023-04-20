using Synonms.RestEasy.Abstractions.Schema;

namespace Synonms.RestEasy.Serialisation.Tests.Unit.Framework;

public static class ResourceFactory
{
    public const bool SomeBool = true;
    public const int SomeInt = 123;
    public const string SomeString = "test";

    public static TestResource Create(Guid id) => new (id, Link.SelfLink(new Uri($"http://localhost:5000/resources/{id}")))
    {
        SomeBool = true, 
        SomeInt = 123, 
        SomeString = "test",
        SomeOptionalString = null
    };

    public static string CreateJson(Guid id) =>
        $@"{{
                ""id"": ""{id}"",
                ""someBool"": {SomeBool.ToString().ToLowerInvariant()},
                ""someInt"": {SomeInt},
                ""someString"": ""{SomeString}"",
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
}