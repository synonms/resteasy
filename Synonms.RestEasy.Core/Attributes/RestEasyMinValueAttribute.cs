namespace Synonms.RestEasy.Core.Attributes;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public sealed class RestEasyMinValueAttribute : Attribute
{
    public RestEasyMinValueAttribute(int minimum)
    {
        DataType = typeof(int);
        Minimum = minimum;
    }

    public RestEasyMinValueAttribute(long minimum)
    {
        DataType = typeof(long);
        Minimum = minimum;
    }

    public RestEasyMinValueAttribute(float minimum)
    {
        DataType = typeof(float);
        Minimum = minimum;
    }

    public RestEasyMinValueAttribute(double minimum)
    {
        DataType = typeof(double);
        Minimum = minimum;
    }

    public RestEasyMinValueAttribute(decimal minimum)
    {
        DataType = typeof(decimal);
        Minimum = minimum;
    }
    
    public Type DataType { get; }
    
    public object Minimum { get; }
}