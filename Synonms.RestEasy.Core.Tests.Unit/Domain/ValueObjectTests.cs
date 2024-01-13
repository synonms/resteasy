using Synonms.RestEasy.Core.Domain;
using Xunit;

namespace Synonms.RestEasy.Core.Tests.Unit.Domain;

public class ValueObjectTests
{
    private record TestIntValueObject(int SomeIntProperty) : ValueObject<int>(SomeIntProperty);
    private record TestStringValueObject(string SomeStringProperty) : ValueObject<string>(SomeStringProperty);
    
    [Theory]
    [InlineData(0, 0)]
    [InlineData(1, 1)]
    [InlineData(int.MinValue, int.MinValue)]
    [InlineData(int.MaxValue, int.MaxValue)]
    public void Equality_IntPropertiesEqual_ReturnsEqual(int left, int right)
    {
        TestIntValueObject leftValueObject = new(left);
        TestIntValueObject rightValueObject = new(right);
        
        Assert.Equal(leftValueObject, rightValueObject);
        Assert.True(leftValueObject == rightValueObject);
        Assert.False(leftValueObject != rightValueObject);
        Assert.True(leftValueObject.Equals(rightValueObject));
    }

    [Theory]
    [InlineData("", "")]
    [InlineData("One", "One")]
    [InlineData(" ", " ")]
    [InlineData("MAXIMUM", "MAXIMUM")]
    public void Equality_StringPropertiesEqual_ReturnsEqual(string left, string right)
    {
        TestStringValueObject leftValueObject = new(left);
        TestStringValueObject rightValueObject = new(right);
        
        Assert.Equal(leftValueObject, rightValueObject);
        Assert.True(leftValueObject == rightValueObject);
        Assert.False(leftValueObject != rightValueObject);
        Assert.True(leftValueObject.Equals(rightValueObject));
    }

    [Theory]
    [InlineData(0, 1)]
    [InlineData(1, 2)]
    [InlineData(int.MaxValue, int.MinValue)]
    public void Equality_IntPropertiesNotEqual_ReturnsNotEqual(int left, int right)
    {
        TestIntValueObject leftValueObject = new(left);
        TestIntValueObject rightValueObject = new(right);
        
        Assert.NotEqual(leftValueObject, rightValueObject);
        Assert.False(leftValueObject == rightValueObject);
        Assert.True(leftValueObject != rightValueObject);
        Assert.False(leftValueObject.Equals(rightValueObject));
    }
    
    [Theory]
    [InlineData("", " ")]
    [InlineData("One", "Two")]
    [InlineData("   ", "       ")]
    public void Equality_StringPropertiesNotEqual_ReturnsNotEqual(string left, string right)
    {
        TestStringValueObject leftValueObject = new(left);
        TestStringValueObject rightValueObject = new(right);
        
        Assert.NotEqual(leftValueObject, rightValueObject);
        Assert.False(leftValueObject == rightValueObject);
        Assert.True(leftValueObject != rightValueObject);
        Assert.False(leftValueObject.Equals(rightValueObject));
    }
}