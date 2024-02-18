using Synonms.RestEasy.Core.Collections;
using Synonms.RestEasy.Core.Schema;

namespace Synonms.RestEasy.WebApi.Collections;

public static class PaginatedListExtensions
{
    public static Pagination GeneratePagination<T>(this PaginatedList<T> paginatedList, Func<int, Uri> resourceCollectionUriFunc) 
    {
        Link firstLink = paginatedList.GenerateFirstLink(resourceCollectionUriFunc);
        Link lastLink = paginatedList.GenerateLastLink(resourceCollectionUriFunc);
        Link? previousLink = paginatedList.GeneratePreviousLink(resourceCollectionUriFunc);
        Link? nextLink = paginatedList.GenerateNextLink(resourceCollectionUriFunc);
        
        return new Pagination(paginatedList.Offset, paginatedList.Limit, paginatedList.Size, firstLink, lastLink)
        {
            Previous = previousLink,
            Next = nextLink
        };
    }
    
    private static Link GenerateFirstLink<T>(this PaginatedList<T> paginatedList, Func<int, Uri> resourceCollectionUriFunc)
    {
        Uri firstUri = resourceCollectionUriFunc(0);
        
        return Link.PageLink(firstUri);
    }

    private static Link GenerateLastLink<T>(this PaginatedList<T> paginatedList, Func<int, Uri> resourceCollectionUriFunc)
    {
        Uri lastUri = resourceCollectionUriFunc(paginatedList.Size - (paginatedList.Size % paginatedList.Limit));
        
        return Link.PageLink(lastUri);
    }

    private static Link? GeneratePreviousLink<T>(this PaginatedList<T> paginatedList, Func<int, Uri> resourceCollectionUriFunc)
    {
        if (paginatedList.HasPrevious is false)
        {
            return null;
        }
        
        Uri previousUri = resourceCollectionUriFunc(Math.Max(paginatedList.Offset - paginatedList.Limit, 0));
        
        return Link.PageLink(previousUri);
    }

    private static Link? GenerateNextLink<T>(this PaginatedList<T> paginatedList, Func<int, Uri> resourceCollectionUriFunc)
    {
        if (paginatedList.HasNext is false)
        {
            return null;
        }
        
        Uri nextUri = resourceCollectionUriFunc(paginatedList.Offset + paginatedList.Limit);
        
        return Link.PageLink(nextUri);
    }
}