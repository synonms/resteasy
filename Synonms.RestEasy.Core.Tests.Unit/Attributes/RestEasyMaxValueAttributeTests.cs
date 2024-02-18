using Synonms.RestEasy.Core.Attributes;
using Xunit;

namespace Synonms.RestEasy.Core.Tests.Unit.Attributes;

public class RestEasyMaxValueAttributeTests
{
    [Fact]
    public void Constructor_Int_SetsProperties()
    {
        const int maximum = 10;
        RestEasyMaxValueAttribute attribute = new(maximum);
        
        Assert.Equal(typeof(int), attribute.DataType);
        Assert.Equal(maximum, attribute.Maximum);
    }

    [Fact]
    public void Constructor_Long_SetsProperties()
    {
        const long maximum = 10;
        RestEasyMaxValueAttribute attribute = new(maximum);
        
        Assert.Equal(typeof(long), attribute.DataType);
        Assert.Equal(maximum, attribute.Maximum);
    }

    [Fact]
    public void Constructor_Double_SetsProperties()
    {
        const double maximum = 10.0d;
        RestEasyMaxValueAttribute attribute = new(maximum);
        
        Assert.Equal(typeof(double), attribute.DataType);
        Assert.Equal(maximum, attribute.Maximum);
    }

    [Fact]
    public void Constructor_Decimal_SetsProperties()
    {
        const decimal maximum = 10.0m;
        RestEasyMaxValueAttribute attribute = new(maximum);
        
        Assert.Equal(typeof(decimal), attribute.DataType);
        Assert.Equal(maximum, attribute.Maximum);
    }
}