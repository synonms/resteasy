using System;
using Synonms.RestEasy.WebApi.Schema;

namespace Synonms.RestEasy.WebApi.Tests.Unit.Serialisation.Shared.Resources;

public static class PaginationFactory
{
    public static readonly Link FirstLink = Link.CollectionLink(new Uri("http://localhost:5000/resources"));
    public static readonly Link LastLink = Link.CollectionLink(new Uri("http://localhost:5000/resources?offset=20"));
    public static readonly Link? PreviousLink = null;
    public static readonly Link? NextLink = Link.CollectionLink(new Uri("http://localhost:5000/resources?offset=10"));

    public const int Offset = 0;
    public const int Limit = 10;
    public const int Size = 25;
    
    public static Pagination Create() =>
        new (Offset, Limit, Size, FirstLink, LastLink)
        {
            Previous = PreviousLink,
            Next = NextLink
        };
}