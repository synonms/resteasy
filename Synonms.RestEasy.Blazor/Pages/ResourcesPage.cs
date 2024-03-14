using Microsoft.AspNetCore.Components;
using Radzen;
using Synonms.Functional;
using Synonms.RestEasy.Blazor.Client;
using Synonms.RestEasy.Blazor.Models;
using Synonms.RestEasy.Core.Schema;
using Synonms.RestEasy.Core.Schema.Resources;

namespace Synonms.RestEasy.Blazor.Pages;

public abstract class ResourcesPage<TResource> : ComponentBase
    where TResource : Resource, new()
{
    protected readonly string CollectionPath;
    protected List<TResource>? Resources;

    protected Pagination? Pagination;

    protected readonly List<BreadcrumbItem> Breadcrumbs;
    
    [Inject] 
    public IRestEasyHttpClient HttpClient { get; set; } = null!;
    
    protected ResourcesPage(string collectionName, string collectionPath)
    {
        CollectionPath = collectionPath;
        Breadcrumbs =
        [
            new BreadcrumbItem(collectionName, $"/{collectionPath}")
        ];
    }
    
    protected override async Task OnInitializedAsync()
    {

        await RefreshResourcesAsync(0);
    }
    
    protected virtual async Task OnPageChanged(PagerEventArgs args)
    {
        await RefreshResourcesAsync(args.Skip);
    }

    protected virtual async Task RefreshResourcesAsync(int offset)
    {
        // TODO: Get uri from service root
        Result<ResourceCollectionDocument<TResource>> response = await HttpClient.GetAllAsync<TResource>(CollectionPath + "?offset=" + offset, CancellationToken.None);

        response.Match(
            resourceCollectionDocument =>
            {
                Resources = resourceCollectionDocument.Resources.ToList();
                Pagination = resourceCollectionDocument.Pagination;
            },
            fault =>
            {
                // TODO: Display errors for client

                Resources = new List<TResource>();
            });
    }
}