using Synonms.RestEasy.Core.Domain.ValueObjects;
using Xunit;

namespace Synonms.RestEasy.Core.Tests.Unit.Domain.ValueObjects;

public class MonikerTests
{
    [Theory]
    [InlineData("One", "One")]
    [InlineData("Two", "Two")]
    [InlineData("MAXIMUM", "MAXIMUM")]
    public void Equality_PropertiesEqual_ReturnsEqual(string left, string right)
    {
        Moniker leftValueObject = Moniker.Convert(left);
        Moniker rightValueObject = Moniker.Convert(right);
        
        Assert.Equal(leftValueObject, rightValueObject);
        Assert.True(leftValueObject == rightValueObject);
        Assert.False(leftValueObject != rightValueObject);
        Assert.True(leftValueObject.Equals(rightValueObject));
    }

    [Theory]
    [InlineData("One", "Two")]
    [InlineData("One", "ONE")]
    [InlineData("One", "  One  ")]
    public void Equality_PropertiesNotEqual_ReturnsNotEqual(string left, string right)
    {
        Moniker leftValueObject = Moniker.Convert(left);
        Moniker rightValueObject = Moniker.Convert(right);
        
        Assert.NotEqual(leftValueObject, rightValueObject);
        Assert.False(leftValueObject == rightValueObject);
        Assert.True(leftValueObject != rightValueObject);
        Assert.False(leftValueObject.Equals(rightValueObject));
    }
}