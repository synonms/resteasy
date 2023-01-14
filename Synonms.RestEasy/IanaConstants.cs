namespace Synonms.RestEasy;

public static class IanaConstants
{
    /// <summary>
    /// https://www.iana.org/assignments/http-methods/http-methods.xhtml
    /// </summary>
    public static class HttpMethods
    {
        public const string Delete = "DELETE";
        public const string Get = "GET";
        public const string Head = "HEAD";
        public const string Options = "OPTIONS";
        public const string Patch = "PATCH";
        public const string Post = "POST";
        public const string Put = "PUT";
        public const string Search = "SEARCH";
    }
    
    /// <summary>
    /// https://www.iana.org/assignments/link-relations/link-relations.xhtml
    /// </summary>
    public static class LinkRelations
    {
        public const string Collection = "collection";
        public const string Item = "item";
        public const string Related = "related";
        public const string Self = "self";
        public const string Service = "service";

        public static class Forms
        {
            public const string Create = "create-form";
            public const string Edit = "edit-form";
        }

        public static class Pagination
        {
            public const string First = "first";
            public const string Last = "last";
            public const string Next = "next";
            public const string Previous = "previous";
        }
    }
}