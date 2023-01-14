namespace Juniper.Catalyst.Rest.Abstractions.Attributes;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public sealed class CatalystMaxValueAttribute : Attribute
{
    public CatalystMaxValueAttribute(int maximum)
    {
        DataType = typeof(int);
        Maximum = maximum;
    }

    public CatalystMaxValueAttribute(long maximum)
    {
        DataType = typeof(long);
        Maximum = maximum;
    }

    public CatalystMaxValueAttribute(float maximum)
    {
        DataType = typeof(float);
        Maximum = maximum;
    }

    public CatalystMaxValueAttribute(double maximum)
    {
        DataType = typeof(double);
        Maximum = maximum;
    }

    public CatalystMaxValueAttribute(decimal maximum)
    {
        DataType = typeof(decimal);
        Maximum = maximum;
    }

    public Type DataType { get; }

    public object Maximum { get; }
}
