namespace Juniper.Catalyst.Rest.Abstractions.Attributes;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public sealed class CatalystMinValueAttribute : Attribute
{
    public CatalystMinValueAttribute(int minimum)
    {
        DataType = typeof(int);
        Minimum = minimum;
    }

    public CatalystMinValueAttribute(long minimum)
    {
        DataType = typeof(long);
        Minimum = minimum;
    }

    public CatalystMinValueAttribute(float minimum)
    {
        DataType = typeof(float);
        Minimum = minimum;
    }

    public CatalystMinValueAttribute(double minimum)
    {
        DataType = typeof(double);
        Minimum = minimum;
    }

    public CatalystMinValueAttribute(decimal minimum)
    {
        DataType = typeof(decimal);
        Minimum = minimum;
    }
    
    public Type DataType { get; }
    
    public object Minimum { get; }
}
