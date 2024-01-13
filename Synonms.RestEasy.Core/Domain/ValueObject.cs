namespace Synonms.RestEasy.Core.Domain;

public abstract record ValueObject
{
}

public abstract record ValueObject<T> : ValueObject
{
    protected ValueObject(T value)
    {
        Value = value;
    }
    
    public T Value { get; private set; }

    public static Type UnderlyingType => typeof(T);
}