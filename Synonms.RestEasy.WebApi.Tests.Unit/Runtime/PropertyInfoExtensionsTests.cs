using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Synonms.RestEasy.WebApi.Constants;
using Synonms.RestEasy.WebApi.Domain;
using Synonms.RestEasy.WebApi.Runtime;
using Synonms.RestEasy.WebApi.Schema.Forms;
using Synonms.RestEasy.WebApi.Tests.Unit.Shared;
using NSubstitute;
using Xunit;

namespace Synonms.RestEasy.WebApi.Tests.Unit.Runtime;

public class PropertyInfoExtensionsTests
{
     private readonly ILookupOptionsProvider _mockLookupOptionsProvider = Substitute.For<ILookupOptionsProvider>();
    
    public PropertyInfoExtensionsTests()
    {
        _mockLookupOptionsProvider
            .Get(Arg.Any<string>())
            .Returns(Enumerable.Empty<FormFieldOption>());
    }
    
    [Theory]
    [InlineData(nameof(TestResource.SomeString), "someString", "string")]
    [InlineData(nameof(TestResource.SomeInt), "someInt", "integer")]
    [InlineData(nameof(TestResource.SomeDecimal), "someDecimal", "decimal")]
    [InlineData(nameof(TestResource.SomeBool), "someBool", "boolean")]
    [InlineData(nameof(TestResource.SomeArray), "someArray", "array")]
    [InlineData(nameof(TestResource.SomeEnumerable), "someEnumerable", "array")]
    [InlineData(nameof(TestResource.SomeDate), "someDate", "date")]
    [InlineData(nameof(TestResource.SomeTime), "someTime", "time")]
    [InlineData(nameof(TestResource.SomeDateTime), "someDateTime", "datetime")]
    [InlineData(nameof(TestResource.SomeTimeSpan), "someTimeSpan", "duration")]
    [InlineData(nameof(TestResource.ChildResources), "childResources", "array")]
    public void CreateFormField_NoCustomAttributes_SetsNameAndTypeOnly(string propertyName, string expectedName, string expectedType)
    {
        TestResource resource = new();

        PropertyInfo propertyInfo = typeof(TestResource).GetProperty(propertyName)
                                              ?? throw new Exception($"Failed to get PropertyInfo for property [{propertyName}].");

        FormField formField = propertyInfo.CreateFormField(resource, _mockLookupOptionsProvider);
        
        Assert.Equal(expectedName, formField.Name);
        Assert.Equal(expectedType, formField.Type);
        Assert.Null(formField.Description);
        Assert.Null(formField.IsEnabled);
        Assert.Null(formField.IsMutable);
        Assert.False(formField.IsRequired);
        Assert.Null(formField.IsSecret);
        Assert.Null(formField.IsVisible);
        Assert.Null(formField.Label);
        Assert.Null(formField.Max);
        Assert.Null(formField.MaxLength);
        Assert.Null(formField.MaxSize);
        Assert.Null(formField.Min);
        Assert.Null(formField.MinLength);
        Assert.Null(formField.MinSize);
        Assert.Null(formField.Pattern);
        Assert.Null(formField.Placeholder);
        Assert.Null(formField.Value);
    }

    [Fact]
    public void CreateFormField_AcceptableValuesAttribute_SetsProperty()
    {
        FormField formField = GetDecoratedPropertyFormField(nameof(TestDecoratedResource.PropertyWithDescriptor));

        Assert.Equal("name", formField.Name);
        Assert.Equal("description", formField.Description);
        Assert.Equal("label", formField.Label);
        Assert.Equal("placeholder", formField.Placeholder);
    }

    [Fact]
    public void CreateFormField_ChildForm_CreatesElementFormFields()
    {
        TestResource resource = new();

        PropertyInfo propertyInfo = typeof(TestResource).GetProperty(nameof(TestResource.ChildResources))
                                    ?? throw new Exception($"Failed to get PropertyInfo for property [{nameof(TestResource.ChildResources)}].");

        FormField formField = propertyInfo.CreateFormField(resource, _mockLookupOptionsProvider);
        
        Assert.Equal("childResources", formField.Name);
        Assert.Equal(DataTypes.Array, formField.Type);
        Assert.Equal(DataTypes.Object, formField.ElementType);
        Assert.NotNull(formField.ElementForm);
        Assert.Contains(formField.ElementForm, x => x.Name == "property1" && x.Type == DataTypes.String);
        Assert.Contains(formField.ElementForm, x => x.Name == "property2" && x.Type == DataTypes.Integer);
    }
    
    [Fact]
    public void CreateFormField_DescriptorAttribute_SetsProperty()
    {
        FormField formField = GetDecoratedPropertyFormField(nameof(TestDecoratedResource.PropertyWithDescriptor));

        Assert.Equal("name", formField.Name);
        Assert.Equal("description", formField.Description);
        Assert.Equal("label", formField.Label);
        Assert.Equal("placeholder", formField.Placeholder);
    }

    [Fact]
    public void CreateFormField_DisabledAttribute_SetsProperty()
    {
        FormField formField = GetDecoratedPropertyFormField(nameof(TestDecoratedResource.PropertyWithDisabled));

        Assert.False(formField.IsEnabled);
    }

    [Fact]
    public void CreateFormField_HiddenAttribute_SetsProperty()
    {
        FormField formField = GetDecoratedPropertyFormField(nameof(TestDecoratedResource.PropertyWithHidden));

        Assert.False(formField.IsVisible);
    }

    [Fact]
    public void CreateFormField_ImmutableAttribute_SetsProperty()
    {
        FormField formField = GetDecoratedPropertyFormField(nameof(TestDecoratedResource.PropertyWithImmutable));

        Assert.False(formField.IsMutable);
    }

    [Fact]
    public void CreateFormField_MaxLengthAttribute_SetsProperty()
    {
        FormField formField = GetDecoratedPropertyFormField(nameof(TestDecoratedResource.PropertyWithMaxLength));

        Assert.Equal(11, formField.MaxLength);
    }

    [Fact]
    public void CreateFormField_MaxSizeAttribute_SetsProperty()
    {
        FormField formField = GetDecoratedPropertyFormField(nameof(TestDecoratedResource.PropertyWithMaxSize));

        Assert.Equal(12, formField.MaxSize);
    }

    [Fact]
    public void CreateFormField_MaxValueAttribute_SetsProperty()
    {
        FormField formField = GetDecoratedPropertyFormField(nameof(TestDecoratedResource.PropertyWithMaxValue));

        Assert.Equal(13, formField.Max);
    }

    [Fact]
    public void CreateFormField_MinLengthAttribute_SetsProperty()
    {
        FormField formField = GetDecoratedPropertyFormField(nameof(TestDecoratedResource.PropertyWithMinLength));

        Assert.Equal(1, formField.MinLength);
    }

    [Fact]
    public void CreateFormField_MinSizeAttribute_SetsProperty()
    {
        FormField formField = GetDecoratedPropertyFormField(nameof(TestDecoratedResource.PropertyWithMinSize));

        Assert.Equal(2, formField.MinSize);
    }

    [Fact]
    public void CreateFormField_MinValueAttribute_SetsProperty()
    {
        FormField formField = GetDecoratedPropertyFormField(nameof(TestDecoratedResource.PropertyWithMinValue));

        Assert.Equal(3, formField.Min);
    }

    [Fact]
    public void CreateFormField_OptionAttributes_SetsProperty()
    {
        FormField formField = GetDecoratedPropertyFormField(nameof(TestDecoratedResource.PropertyWithOptions));

        Assert.NotNull(formField.Options);
        Assert.Collection(formField.Options,
            x =>
            {
                Assert.Equal(1, x.Value);
                Assert.Equal("one", x.Label);
            },
            x =>
            {
                Assert.Equal(2, x.Value);
                Assert.Equal("two", x.Label);
            });
    }

    [Fact]
    public void CreateFormField_PatternAttribute_SetsProperty()
    {
        FormField formField = GetDecoratedPropertyFormField(nameof(TestDecoratedResource.PropertyWithPattern));

        Assert.Equal("pattern", formField.Pattern);
    }
    
    [Fact]
    public void CreateFormField_RequiredAttribute_SetsProperty()
    {
        FormField formField = GetDecoratedPropertyFormField(nameof(TestDecoratedResource.PropertyWithRequired));

        Assert.True(formField.IsRequired);
    }

    [Fact]
    public void CreateFormField_SecretAttribute_SetsProperty()
    {
        FormField formField = GetDecoratedPropertyFormField(nameof(TestDecoratedResource.PropertyWithSecret));

        Assert.True(formField.IsSecret);
    }

    [Fact]
    public void CreateFormField_SimplePropertyWithAllAttributes_SetsAppropriateProperties()
    {
        FormField formField = GetDecoratedPropertyFormField(nameof(TestDecoratedResource.SimplePropertyWithAllAttributes));

        Assert.Equal("name", formField.Name);
        Assert.Equal("description", formField.Description);
        Assert.Null(formField.Form);
        Assert.Equal(false, formField.IsEnabled);
        Assert.Equal(false, formField.IsMutable);
        Assert.Equal(true, formField.IsRequired);
        Assert.Equal(true, formField.IsSecret);
        Assert.Equal(false, formField.IsVisible);
        Assert.Equal("label", formField.Label);
        Assert.Equal(300, formField.Max);
        Assert.Equal(100, formField.MaxLength);
        Assert.Equal(30, formField.Min);
        Assert.Equal(10, formField.MinLength);
        Assert.NotNull(formField.Options);
        Assert.Collection(formField.Options,
            x =>
            {
                Assert.Equal(1, x.Value);
                Assert.Equal("one", x.Label);
            },
            x =>
            {
                Assert.Equal(2, x.Value);
                Assert.Equal("two", x.Label);
            });
        Assert.Equal("pattern", formField.Pattern);
        Assert.Equal("placeholder", formField.Placeholder);
        Assert.Equal(DataTypes.Integer, formField.Type);
        Assert.Equal(123, formField.Value);
    }

    [Fact]
    public void CreateFormField_EnumerablePropertyWithAllAttributes_SetsAppropriateProperties()
    {
        FormField formField = GetDecoratedPropertyFormField(nameof(TestDecoratedResource.EnumerablePropertyWithAllAttributes));

        Assert.Equal("name", formField.Name);
        Assert.Equal("description", formField.Description);
        Assert.Equal(DataTypes.String, formField.ElementType);
        Assert.NotNull(formField.ElementForm);
        Assert.Collection(formField.ElementForm!,
            x =>
            {
                Assert.Equal(300, x.Max);
                Assert.Equal(100, x.MaxLength);
                Assert.Equal(30, x.Min);
                Assert.Equal(10, x.MinLength);
                Assert.NotNull(x.Options);
                Assert.Collection(x.Options,
                    o =>
                    {
                        Assert.Equal(1, o.Value);
                        Assert.Equal("one", o.Label);
                    },
                    o =>
                    {
                        Assert.Equal(2, o.Value);
                        Assert.Equal("two", o.Label);
                    });
                Assert.Equal("pattern", x.Pattern);
                Assert.Equal("placeholder", x.Placeholder);
                Assert.Equal(DataTypes.String, x.Type);
            });
        Assert.Equal(false, formField.IsEnabled);
        Assert.Equal(false, formField.IsMutable);
        Assert.Equal(true, formField.IsRequired);
        Assert.Equal(false, formField.IsVisible);
        Assert.Equal("label", formField.Label);
        Assert.Equal(200, formField.MaxSize);
        Assert.Equal(20, formField.MinSize);
        Assert.Equal(DataTypes.Array, formField.Type);
        Assert.NotNull(formField.Value);
        IEnumerable<string>? formFieldStringValues = formField.Value as IEnumerable<string>;
        Assert.NotNull(formFieldStringValues);
        Assert.Collection(formFieldStringValues,
            x => Assert.Equal("one", x), 
            x => Assert.Equal("two", x));
    }

    private FormField GetDecoratedPropertyFormField(string propertyName)
    {
        TestDecoratedResource resource = new();

        PropertyInfo propertyInfo = typeof(TestDecoratedResource).GetProperty(propertyName)
            ?? throw new Exception($"Failed to get PropertyInfo for property [{propertyName}].");
        
        return propertyInfo.CreateFormField(resource, _mockLookupOptionsProvider);
    }
}