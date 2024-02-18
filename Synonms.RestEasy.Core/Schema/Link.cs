using Synonms.RestEasy.Core.Constants;

namespace Synonms.RestEasy.Core.Schema;

public class Link
{
    public Link(Uri uri, string relation, string method)
    {
        Uri = uri;
        Relation = relation;
        Method = method;
    }

    public Uri Uri { get; }
    
    public string Relation { get; }
    
    public string Method { get; }
    
    public string[]? Accepts { get; init; }

    public static Link CollectionLink(Uri uri) =>
        new (uri, IanaLinkRelations.Collection, IanaHttpMethods.Get);

    public static Link CreateFormLink(Uri uri) => 
        new (uri, IanaLinkRelations.Forms.Create, IanaHttpMethods.Get);
    
    public static Link CreateFormTargetLink(Uri uri) =>
        new (uri, IanaLinkRelations.Forms.Create, IanaHttpMethods.Post);

    public static Link EditFormLink(Uri uri) => 
        new (uri, IanaLinkRelations.Forms.Edit, IanaHttpMethods.Get);
    
    public static Link EditFormTargetLink(Uri uri) =>
        new (uri, IanaLinkRelations.Forms.Edit, IanaHttpMethods.Put);

    public static Link EmptyLink() =>
        new (new Uri("http://localhost"), string.Empty, string.Empty);

    public static Link ItemLink(Uri uri) =>
        new (uri, IanaLinkRelations.Item, IanaHttpMethods.Get);

    public static Link DeleteSelfLink(Uri uri) =>
        new (uri, IanaLinkRelations.Self, IanaHttpMethods.Delete);

    public static Link PageLink(Uri uri) =>
        new (uri, IanaLinkRelations.Collection, IanaHttpMethods.Get);
    
    public static Link RelationLink(Uri uri) =>
        new (uri, IanaLinkRelations.Related, IanaHttpMethods.Get);
    
    public static Link SelfLink(Uri uri) =>
        new (uri, IanaLinkRelations.Self, IanaHttpMethods.Get);
}