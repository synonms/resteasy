namespace Synonms.RestEasy;

public static class RestEasyConstants
{
    public static class PropertyNames
    {
        public static class Error
        {
            public const string Id = "id";
        }

        public static class ResourceIdentifier
        {
            public const string Id = "id";
            public const string Type = "type";
        }
    }
    
    public static class HttpContextItemKeys
    {
        public const string ApiVersion = "ApiVersion";
        public const string CorrelationId = "CorrelationId";
        public const string RequestId = "RequestId";
    }

    public static class HttpHeaders
    {
        public const string ApiVersion = "X-Catalyst-ApiVersion";
        public const string CorrelationId = "X-Correlation-ID";
        public const string RequestId = "X-Request-ID";
    }

    public static class HttpHeaderKeys
    {
        public const string ApiVersion = "apiVersion";
    }

    public static class HttpQueryStringKeys
    {
        public const string ApiVersion = "apiVersion";
    }

    public static class OpenApiStringFormats
    {
        public const string Binary = "binary";
        public const string Byte = "byte";
        public const string Date = "date";
        public const string DateTime = "date-time";
        public const string Email = "email";
        public const string HostName = "hostname";
        public const string Password = "password";
        public const string Uri = "uri";
        public const string Uuid = "uuid";
    }

    public static class OpenApiDataTypes
    {
        public const string Array = "array";
        public const string Boolean = "boolean";
        public const string Integer = "integer";
        public const string Object = "object";
        public const string String = "string";
        public const string Number = "number";
    }

    public static class Versioning
    {
        public const int Default = 1;
    }
}