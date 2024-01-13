using System;
using System.Collections.Generic;
using System.Linq;
using Synonms.RestEasy.WebApi.Constants;
using Synonms.RestEasy.WebApi.Domain;
using Synonms.RestEasy.WebApi.Schema.Forms;
using Synonms.RestEasy.WebApi.Schema.Resources;
using Synonms.RestEasy.WebApi.Tests.Unit.Shared;
using NSubstitute;
using Xunit;

namespace Synonms.RestEasy.WebApi.Tests.Unit.Schema.Resources;

public class ResourceExtensionsTests
{
    private readonly ILookupOptionsProvider _mockLookupOptionsProvider = Substitute.For<ILookupOptionsProvider>();
    
    public ResourceExtensionsTests()
    {
        _mockLookupOptionsProvider
            .Get(Arg.Any<string>())
            .Returns(Enumerable.Empty<FormFieldOption>());
    }

    [Fact]
    public void GenerateCreateForm_AddsFormFieldsAndSetsTargetLink()
    {
        TestResource resource = new();
        Uri targetUri = new(TestRouting.UriBasePath);
        
        Form createForm = resource.GenerateCreateForm<TestAggregateRoot, TestResource>(targetUri, _mockLookupOptionsProvider);

        Assert.Equal(10, createForm.Fields.Count());

        Assert.Equal(targetUri, createForm.Target.Uri);
        Assert.Equal(IanaLinkRelations.Forms.Create, createForm.Target.Relation);
        Assert.Equal(IanaHttpMethods.Post, createForm.Target.Method);
    }
    
    [Fact]
    public void GenerateEditForm_AddsFormFieldsAndSetsTargetLink()
    {
        TestResource resource = new();
        Uri targetUri = new(TestRouting.UriBasePath);
        
        Form editForm = resource.GenerateEditForm<TestAggregateRoot, TestResource>(targetUri, _mockLookupOptionsProvider);

        Assert.Equal(10, editForm.Fields.Count());

        Assert.Equal(targetUri, editForm.Target.Uri);
        Assert.Equal(IanaLinkRelations.Forms.Edit, editForm.Target.Relation);
        Assert.Equal(IanaHttpMethods.Put, editForm.Target.Method);
    }
}