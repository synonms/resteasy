namespace Synonms.RestEasy.Abstractions.Attributes;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public sealed class RestEasyMaxValueAttribute : Attribute
{
    public RestEasyMaxValueAttribute(int maximum)
    {
        DataType = typeof(int);
        Maximum = maximum;
    }

    public RestEasyMaxValueAttribute(long maximum)
    {
        DataType = typeof(long);
        Maximum = maximum;
    }

    public RestEasyMaxValueAttribute(float maximum)
    {
        DataType = typeof(float);
        Maximum = maximum;
    }

    public RestEasyMaxValueAttribute(double maximum)
    {
        DataType = typeof(double);
        Maximum = maximum;
    }

    public RestEasyMaxValueAttribute(decimal maximum)
    {
        DataType = typeof(decimal);
        Maximum = maximum;
    }

    public Type DataType { get; }

    public object Maximum { get; }
}
