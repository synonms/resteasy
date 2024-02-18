using System.Text.Json;
using Synonms.RestEasy.Core.Constants;
using Synonms.RestEasy.Core.Schema;
using Synonms.RestEasy.Core.Serialisation.Ion;
using Synonms.RestEasy.WebApi.Tests.Unit.Serialisation.Shared.Resources;
using Xunit;

namespace Synonms.RestEasy.WebApi.Tests.Unit.Serialisation.Shared.Ion;

public static class IonPaginationAssertions
{
    public static void Verify(JsonElement resourceElement)
    {
        Assert.Equal(PaginationFactory.Offset, resourceElement.GetProperty(IonPropertyNames.Pagination.Offset).GetInt32());
        Assert.Equal(PaginationFactory.Limit, resourceElement.GetProperty(IonPropertyNames.Pagination.Limit).GetInt32());
        Assert.Equal(PaginationFactory.Size, resourceElement.GetProperty(IonPropertyNames.Pagination.Size).GetInt32());
        
        Assert.Equal(PaginationFactory.FirstLink.Uri.OriginalString, resourceElement.GetProperty(IanaLinkRelations.Pagination.First).GetProperty(IonPropertyNames.Links.Uri).GetString());
        Assert.Equal(PaginationFactory.LastLink.Uri.OriginalString, resourceElement.GetProperty(IanaLinkRelations.Pagination.Last).GetProperty(IonPropertyNames.Links.Uri).GetString());
        Assert.Equal(PaginationFactory.NextLink?.Uri.OriginalString, resourceElement.GetProperty(IanaLinkRelations.Pagination.Next).GetProperty(IonPropertyNames.Links.Uri).GetString());
        Assert.False(resourceElement.TryGetProperty(IanaLinkRelations.Pagination.Previous, out JsonElement _));
    }

    public static void Verify(Pagination pagination)
    {
        Assert.Equal(PaginationFactory.Offset, pagination.Offset);
        Assert.Equal(PaginationFactory.Limit, pagination.Limit);
        Assert.Equal(PaginationFactory.Size, pagination.Size);

        Assert.Equal(PaginationFactory.FirstLink.Uri, pagination.First.Uri);
        Assert.Equal(PaginationFactory.FirstLink.Method, pagination.First.Method);
        Assert.Equal(PaginationFactory.FirstLink.Relation, pagination.First.Relation);
        
        Assert.Equal(PaginationFactory.LastLink.Uri, pagination.Last.Uri);
        Assert.Equal(PaginationFactory.LastLink.Method, pagination.Last.Method);
        Assert.Equal(PaginationFactory.LastLink.Relation, pagination.Last.Relation);
        
        Assert.Equal(PaginationFactory.NextLink?.Uri, pagination.Next?.Uri);
        Assert.Equal(PaginationFactory.NextLink?.Method, pagination.Next?.Method);
        Assert.Equal(PaginationFactory.NextLink?.Relation, pagination.Next?.Relation);

        Assert.Null(pagination.Previous);
    }
}