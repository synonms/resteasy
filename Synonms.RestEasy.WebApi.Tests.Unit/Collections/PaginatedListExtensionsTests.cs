using System;
using System.Collections.Generic;
using Synonms.RestEasy.Core.Collections;
using Synonms.RestEasy.Core.Schema;
using Synonms.RestEasy.WebApi.Collections;
using Synonms.RestEasy.WebApi.Tests.Unit.Shared;
using Xunit;

namespace Synonms.RestEasy.WebApi.Tests.Unit.Collections;

public class PaginatedListExtensionsTests
{
    [Fact]
    public void GeneratePagination_SetsCorrectProperties()
    {
        List<TestAggregateRoot> aggregateRoots = new()
        {
            new TestAggregateRoot(),
            new TestAggregateRoot(),
            new TestAggregateRoot(),
            new TestAggregateRoot()
        };
        // 10 in total: 2 skipped, 4 this page, 4 next page
        PaginatedList<TestAggregateRoot> paginatedList = new (aggregateRoots, 2, 4, 10);

        Uri PaginationUriFunc(int offset)
        {
            string queryString = offset > 0 ? "?offset=" + offset : string.Empty;
            return new Uri(TestRouting.UriBasePath + queryString);
        }
        
        Pagination pagination = paginatedList.GeneratePagination(PaginationUriFunc);
        
        Assert.Equal(2, pagination.Offset);
        Assert.Equal(4, pagination.Limit);
        Assert.Equal(10, pagination.Size);
        Assert.Equal(TestRouting.UriBasePath, pagination.First.Uri.OriginalString);
        Assert.Equal(TestRouting.UriBasePath + "?offset=8", pagination.Last.Uri.OriginalString);
        Assert.NotNull(pagination.Previous);
        Assert.Equal(TestRouting.UriBasePath, pagination.Previous?.Uri.OriginalString);
        Assert.NotNull(pagination.Next);
        Assert.Equal(TestRouting.UriBasePath + "?offset=6", pagination.Next?.Uri.OriginalString);
    }
}